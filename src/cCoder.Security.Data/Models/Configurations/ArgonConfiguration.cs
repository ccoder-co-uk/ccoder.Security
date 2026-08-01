// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Security.Models.Configurations;

public class ArgonConfiguration
{
    public ArgonConfiguration()
    {
        MemorySizeInKilobytes = 19_456;
        Iterations = 2;
        DegreeOfParallelism = 1;
        SaltSizeInBytes = 16;
        HashSizeInBytes = 32;
    }

    public int MemorySizeInKilobytes { get; set; }
    public int Iterations { get; set; }
    public int DegreeOfParallelism { get; set; }
    public int SaltSizeInBytes { get; set; }
    public int HashSizeInBytes { get; set; }
}