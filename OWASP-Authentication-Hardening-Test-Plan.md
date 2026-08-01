# OWASP Authentication Hardening Test Plan

## Objective

Establish an executable security baseline for cCoder authentication before adding external identity providers or OAuth 2.0 flows. The baseline targets OWASP ASVS level 2 expectations and the OWASP Authentication, Password Storage, Session Management, and Forgot Password guidance.

Passing this plan means that authentication behavior is proved at the service, HTTP API, and composed-application boundaries. A filename or implementation review is not evidence on its own.

## Ownership

| Boundary | Repository | Evidence |
|---|---|---|
| Password storage, account lifecycle, tokens, lockout, server-side authorization | `cCoder.Security` | Unit, integration, and API acceptance tests |
| Composed HTTP application, cookies, headers, throttling, browser-facing contracts | `cCoder.Applications` | Isolated penetration and standardisation tests |
| Deterministic source-code anti-patterns | `cCoder.CodeAnalysis` | Analyzer positive and negative tests |

## Release gates

### Password storage

- New passwords are stored only with the ASP.NET Core Identity password hasher or a stronger approved adaptive password hasher.
- Two users choosing the same password receive different stored hashes.
- Stored values never contain the plaintext password and cannot be decrypted.
- Raw SHA-family hashes and reversible encryption cannot be selected for new password storage.
- Existing legacy encrypted values, if migration support is required, are accepted only for verification and immediately replaced with the approved one-way hash after successful authentication.
- `SuccessRehashNeeded` causes the stored hash to be upgraded.
- New passwords use Argon2id with a versioned stored representation. Runtime configuration is rejected below OWASP's minimum of 19 MiB memory, 2 iterations, and 1 lane; salts are at least 128 bits and derived hashes at least 256 bits. Tests protect every minimum from accidental regression.
- Existing ASP.NET Core Identity V3 PBKDF2-HMAC-SHA512 hashes remain verifiable only as a migration format and are replaced with Argon2id after successful authentication.
- Registration, password change, invitation acceptance, and password reset all use the same central hashing path.
- Password validation permits long passwords and all Unicode characters, rejects unreasonably large inputs safely, and applies the agreed minimum length without silently truncating values.

### Authentication and enumeration resistance

- Unknown user, wrong password, locked user, disabled/unconfirmed user, and malformed credentials produce the same public login status and message.
- Registration and password-recovery responses do not reveal whether an email or username exists.
- The unknown-user path performs equivalent password-hash work so it is not a trivial timing oracle.
- Authentication failures do not return exception messages, stack traces, entity values, hashes, tokens, or internal identifiers.
- Authentication controls are server-side and every protected endpoint rejects absent, expired, revoked, or wrong-purpose credentials.

### Automated attack resistance

- Failed attempts are counted against the account independently of source address.
- The lock threshold is tested at the exact boundary: the last allowed attempt remains allowed and the next is rejected.
- Lockout has a bounded duration and expires; it must not create a permanent denial-of-service condition.
- Successful authentication resets the failure count only when the account is not currently locked.
- Login, registration, and password-recovery endpoints have request throttling in the composed application.
- Penetration tests exercise per-account and per-origin/IP limits independently and assert a generic `429` response without sensitive diagnostics.
- Concurrent failed attempts cannot bypass the threshold through lost updates.

### Recovery, confirmation, and authentication tokens

- Tokens are generated with a cryptographically secure random source and sufficient entropy.
- Bearer tokens use a random selector plus at least 256 random secret bits and expose only the combined opaque Base64URL value to the caller. `IPasswordHashingUtilityBroker` provides distinct operations: slow Argon2id derivation for human passwords, and fast cryptographic hashing with fixed-time comparison for already-high-entropy token secrets. The non-secret selector supports indexed lookup without applying a password KDF to every authenticated request.
- Tokens are purpose-bound, user-bound, time-bound, and single-use.
- Expired, consumed, unknown, wrong-user, and wrong-purpose tokens are rejected identically.
- Issuing a new recovery token invalidates older recovery tokens for that user, or the chosen alternative policy is explicitly tested.
- Password reset invalidates existing authentication sessions/tokens and clears a legitimate timed lockout.
- Password changes require the current password and invalidate other active authentication sessions.
- Logout revokes the active token and clears the session.
- Tokens and passwords never appear in application logs.
- An authorised administrator has an explicit, audited recovery operation for an account whose recovery channel is unavailable; it must not use a default credential, reveal or decrypt the old password, or bypass the normal password hasher.

### Session and browser boundary

- Session cookies are `Secure`, `HttpOnly`, and have an explicit restrictive `SameSite` policy.
- Session identifiers rotate on login and other privilege changes, and are invalid after logout.
- Authentication responses are not cached by browsers or intermediary caches.
- State-changing authenticated requests have CSRF protection appropriate to the authentication mechanism.
- Redirect targets are local/allow-listed and cannot be used for open redirects.
- Password inputs use the correct HTML password type and autocomplete purpose (`current-password` or `new-password`).
- UI behavior preserves the generic server response and does not reconstruct account-enumeration messages.

### Authorization boundary

- Anonymous callers cannot reach protected account, tenant, role, app, or platform administration operations.
- Authenticated users cannot cross tenant/app boundaries or assign themselves elevated roles.
- Privileged account-management operations require the expected role and recent authentication where applicable.
- Authentication provider callbacks cannot create or link an account until issuer, audience, signature, state, nonce, redirect URI, and provider identity have been validated. These become mandatory tests before the first OAuth/provider release.

## Code-analysis scope

Static analysis should enforce only deterministic source facts with low false-positive risk:

- Keep `OWASP0001`: HTTP handlers must not expose exception details.
- Flag password storage performed with reversible encryption APIs.
- Flag direct fast hashes (`MD5`, `SHA1`, `SHA256`, `SHA384`, or `SHA512`) when the value is assigned to a password/hash property or implemented by a password-storage broker.
- Flag plaintext password/token values written through recognised logging calls.
- Flag password model properties exposed by OData/API entity models unless explicitly ignored or represented by an input-only DTO.

Runtime characteristics such as entropy, single use, throttling behavior, response uniformity, timing, cookie rotation, and authorization outcomes belong in executable tests rather than analyzer heuristics.

## Current baseline findings

- `OWASP0001` currently covers exception-detail disclosure and has one positive rule test; it needs a negative test and representative handler samples.
- `cCoder.Security` has useful happy-path lifecycle coverage and one lockout integration test, but the lockout is currently permanent after eleven failures and has no independent request-throttling coverage.
- `cCoder.Security` currently selects reversible AES-backed password storage when `Security:DecryptionKey` is configured. Raw SHA-512 storage is also selectable. Both paths fail this plan.
- The ASP.NET Core Identity `PasswordHasher` implementation exists, but `SuccessRehashNeeded` is treated as ordinary success and does not update the stored value.
- Password recovery has a generic public HTTP response, but service behavior and timing-equivalence coverage remain incomplete.
- `cCoder.Applications` has valuable generic penetration coverage for disclosure, headers, cookies, CORS, traversal, and injection, but no authentication lifecycle, throttling, session rotation, CSRF, or enumeration-resistance penetration suite.

## Execution order

1. Make the approved adaptive password hasher the only path for newly stored passwords, with narrowly scoped legacy verification-and-rehash support if production data requires it.
2. Add focused Security unit tests for hashing, response uniformity, exact lockout boundaries, lock expiry, concurrency, and token rules.
3. Add Security integration/acceptance lifecycle tests for every externally observable authentication outcome.
4. Add composed Applications penetration tests for throttling, cookies/session rotation, caching, CSRF, redirects, and public-response uniformity.
5. Add narrow CodeAnalysis diagnostics and positive/negative samples for password-storage and secret-logging anti-patterns.
6. Run all three repositories' test suites and retain a requirement-to-test evidence table as the release gate for OAuth/provider work.
