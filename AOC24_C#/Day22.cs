using System.Diagnostics;
using System.Dynamic;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Day22;



/*

s * 64, mix, prune
s/32, mix, prune
s*2048, mix, prune

mix: s = s xor a
prune : s = s modulo 16777216

*/

class Day22
{

    public static void ComputeSecretSIMD(ref Vector<long> secret, int iter)
    {
        Vector<long> result;
        // 16777215 = 2^24 - 1 (equivalent to % 16777216)
        Vector<long> moduloMask = new(16777215);

        for (int i = 0; i < iter; i++)
        {
            // long result = secret * 64;
            // long result = secret << 6;
            // secret ^= result;
            // secret %= 16777216;
            result = Vector.ShiftLeft(secret, 6);
            secret = Vector.Xor(result, secret);
            secret = Vector.BitwiseAnd(secret, moduloMask); 

            // // result = secret / 32;
            // result = secret >> 5;
            // secret ^= result;
            // secret %= 16777216;
            result = Vector.ShiftRightLogical(secret, 5);
            secret = Vector.Xor(result, secret);
            secret = Vector.BitwiseAnd(secret, moduloMask); 
            
            // // result = secret * 2048;
            // result = secret << 11;
            // secret ^= result;
            // secret %= 16777216;
            result = Vector.ShiftLeft(secret, 11);
            secret = Vector.Xor(result, secret);
            secret = Vector.BitwiseAnd(secret, moduloMask); 
        }
    }

    public static long ComputeSecret(long secret, int iter)
    {
        for (int i = 0; i < iter; i++)
        {

            // long result = secret * 64;
            long result = secret << 6;
            secret ^= result;
            // secret %= 16777216;
            secret &= 16777215;

            // result = secret / 32;
            result = secret >> 5;
            secret ^= result;
            // secret %= 16777216;
            secret &= 16777215;
            
            // result = secret * 2048;
            result = secret << 11;
            secret ^= result;
            // secret %= 16777216;
            secret &= 16777215;
        }

        return secret;
    }

    public static long[] ParseInput(string filePath = @"..\..\..\input_22.txt")
    {
        using StreamReader sr = File.OpenText(filePath);
        var seeds = sr.ReadToEnd().Split('\n',StringSplitOptions.RemoveEmptyEntries)
                        .Select(long.Parse).ToArray();
        
        return seeds;
    }

    public static long NaiveSolution(ref long[] seeds, int iter)
    {        
        long total = 0;
        for (int i = 0; i < iter; i++)
        {
            total += ComputeSecret(seeds[i], iter);
        }

        return total;
    }

    public static long ParallelSolution(ref long[] seeds, int iter)
    {
        // long total = seeds.AsParallel().Sum(seed => ComputeSecret(seed, 2000)); 
        long total = 0L;
        Parallel.ForEach(seeds, seed => 
        {
            Interlocked.Add(ref total, ComputeSecret(seed, iter));
        });

        return total;
    }

    public static long SIMDSolution(ref long[] seeds, int iter)
    {
    
        var vectorSize = Vector<long>.Count;
        long total = 0;
        
        Span<long> seedSpan = seeds.AsSpan();
        int i;
        for (i = 0; i <= seeds.Length - vectorSize; i += vectorSize)
        {
            // Vector<long> secrets = new(seeds, i);
            Vector<long> secrets = MemoryMarshal.Cast<long, Vector<long>>(seedSpan.Slice(i))[0];
            ComputeSecretSIMD(ref secrets, iter);
            total += Vector.Dot(Vector<long>.One, secrets);
        }

        for (; i < seeds.Length; i++) {
            total += ComputeSecret(seeds[i], iter);
        }


        return total;
    }



    public static long Part1()
    {
        var seeds = ParseInput();
        Stopwatch watch = Stopwatch.StartNew();
        var result = NaiveSolution(ref seeds, 2000);
        Debug.Assert(result == 17_965_282_217); 
        watch.Stop();
        Console.WriteLine($"NAIVE: {watch.ElapsedMilliseconds}ms");
        return result;
    }


    public static long[] ComputeAllStepsBuyer(long secret, int iter)
    {
        long[] list = new long[iter + 1];
        list[0] = secret % 10;

        for (int i = 1; i < iter + 1; i++)
        {
            secret = ComputeSecret(secret, 1);
            list[i] = secret % 10; // Only add rightmost digit
        }
        return list;
    }

    public static long[][] ComputeAllStepsAllBuyers(long[] secrets, int iter)
    {
        long[][] steps = new long[secrets.Length][];

        for (int i = 0; i < secrets.Length; i++)
            steps[i] = ComputeAllStepsBuyer(secrets[i], iter);

        return steps;
    }
    



    public static Dictionary<(long, long, long, long), long> GenerateAllSequences(long[][] buyerSteps)
    {
        Dictionary<(long, long, long, long), long> allSequences = [];
        HashSet<(long,long,long,long)> visitedSequences = [];
        Queue<long> last4Diffs = [];

        foreach (var buyerStep in buyerSteps)
        {
            long? lastPrice = null;
            last4Diffs.Clear();
            visitedSequences.Clear();

            foreach (var currentPrice in buyerStep)
            {
                if (!lastPrice.HasValue) {
                    lastPrice = currentPrice;
                    continue;
                }
                
                long diff = currentPrice - lastPrice.Value;
                last4Diffs.Enqueue(diff);

                if (last4Diffs.Count == 4)
                {
                    var s = last4Diffs.ToArray();
                    var key = ( s[0], s[1], s[2], s[3] );

                    if (!visitedSequences.Contains(key))
                    {
                        visitedSequences.Add(key);
                        allSequences.TryAdd(key, 0);
                        allSequences[key] += currentPrice;
                    }
                    
                    last4Diffs.Dequeue();
                }

                lastPrice = currentPrice;
            }
        }

        return allSequences;
    }   

    public static long Part2()
    {
        var seeds = ParseInput(@"..\..\..\input_22.txt");
        var buyerSteps = ComputeAllStepsAllBuyers(seeds, 2000);

        long max = -1;        
        var sequences = GenerateAllSequences(buyerSteps);
        (long, long, long, long) bestSeq = (0,0,0,0);

        foreach (var s in sequences.Keys)
        {
            if (sequences[s] > max)
            {
                bestSeq = s;
                max = sequences[s];
            }
        }

        Console.WriteLine(bestSeq);
        return max;

    }
}