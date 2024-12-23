namespace Day9;


struct FileInfo(long fileID, long blocks, int index)
{
    public long FileID = fileID;
    public long Blocks = blocks;

    public int Index = index;

}


class DiskMap 
{

    private static readonly long EMPTY_BLOCK = -1;
    private readonly long[] rawData;
    private readonly List<long> blocks;

    public DiskMap(string inputFile)
    {
        using StreamReader sr = File.OpenText(inputFile);
        rawData = [.. sr.ReadToEnd().Select(x => long.Parse(x.ToString()))];
        blocks = [];
        GenerateBlocks();
    }

    public void PrintBlocks()
    {
        blocks.ForEach(x => Console.Write(x == EMPTY_BLOCK? "." : x));
        Console.WriteLine();
    }

    private void AddBlock(long fileID, long quantity)
    {
        for (long i = 0; i < quantity; i++)
        {
            this.blocks.Add(fileID);
        }
    }

    private void AddEmptyBlock(long quantity)
    {
        for (long i = 0; i < quantity; i++)
        {
            this.blocks.Add(EMPTY_BLOCK);
        }
    }


    public void MoveFile(FileInfo emptyBlock, FileInfo file)
    {
        int fileIdx = file.Index;
        int emptyIdx = emptyBlock.Index;

        for (int i = 0; i < file.Blocks; i++)
        {
            blocks[emptyIdx + i] = blocks[fileIdx - i];
            blocks[fileIdx - i] = EMPTY_BLOCK;
        }
    }

    public IEnumerable<FileInfo> IterateFilesFromRight()
    {
        int END = blocks.Count - 1;
        int right = END;

    
        while (right >= 0)
        {
            while (right >= 0  && blocks[right] == EMPTY_BLOCK)
            {
                right --;
            }

            // Check not finished
            long fileID = blocks[right]; 
            int idx = right;
            long quantity = 0;

            while (right >= 0 && blocks[right] == fileID)
            {
                quantity ++;
                right --;
            }
            
            yield return new (fileID, quantity, idx);

            // Console.WriteLine($"Found file {fileID} x {quantity} blocks");

        }
    }


    public IEnumerable<FileInfo> IterateEmptyBlocks()
    {
        int END = blocks.Count - 1;
        int left = 0;

    
        while (left < END)
        {
            while (left < END  && blocks[left] != EMPTY_BLOCK)
            {
                left ++;
            }
 
            int idx = left;
            long quantity = 0;

            while (left < END && blocks[left] == EMPTY_BLOCK)
            {
                quantity ++;
                left ++;
            }
            
            if (quantity > 0)
            {
                // Console.WriteLine($"Found empty block at {idx} size {quantity}");
                yield return new (EMPTY_BLOCK, quantity, idx);

            }
        }
    }


    public void DefragmentFullFiles()
    {
        var files = IterateFilesFromRight().ToList();
        var emptyBlocks = IterateEmptyBlocks().ToList();

        foreach (var file in files)
        {
            for (int emptyIdx = 0; emptyIdx < emptyBlocks.Count; emptyIdx++)
            {
                var emptyBlock = emptyBlocks[emptyIdx];

                if (emptyBlock.Blocks >= file.Blocks)
                {

                    long fileLeft =  file.Index - file.Blocks - 1;

                    if (fileLeft > emptyBlock.Index){
                        MoveFile(emptyBlock: emptyBlock, file: file);
                        

                        long occupiedBlocks = file.Blocks;

                        FileInfo updatedEmpty = new(
                            fileID: emptyBlock.FileID,
                            blocks: emptyBlock.Blocks - file.Blocks,
                            index: emptyBlock.Index + (int)occupiedBlocks
                        );

                        emptyBlocks[emptyIdx] = updatedEmpty;
                        break;
                    }

                }
            }
        }      
    }

    public void RemoveEmptySpace()
    {

        int END = blocks.Count - 1;

        int left = 0,
            right = END;

        while (left < right && left < END && right >= 0)
        {
            // Move left to next empty block
            while (left < END && blocks[left] != EMPTY_BLOCK)
            {
                left++;
            }

           
            // Move right to next fileID
            while (right >= 0 && blocks[right] == EMPTY_BLOCK)
            {
                right --;
            }

            if (left < right)
            {   
                // Swap left and right
                (blocks[right], blocks[left]) = (blocks[left], blocks[right]);
            }

            PrintBlocks();
        }

    }


    public long GetChecksum()
    {
        long checksum = 0;
        long blockID = 0;

        foreach (var fileID in blocks)
        {
            // if (fileID == EMPTY_BLOCK) break;

            if (fileID != EMPTY_BLOCK)
            {
                checksum += fileID * blockID;
            }

            blockID ++;
        }

        return checksum;
    }

    private void GenerateBlocks() 
    {

        long fileID = 0;
        for (long i = 0; i < rawData.Length; i++) 
        {
            long quantity = int.Parse(rawData[i].ToString());
            bool isFile = i % 2 == 0;
            if (isFile)
            {
                AddBlock(fileID, quantity);
                fileID ++;
            }
            else
            {
                AddEmptyBlock(quantity);
            }
        }
    }

}


class Day9
{
    private static readonly string inputFile = @"..\..\..\input_9.txt"; 


    public static long Part1()
    {
        DiskMap fs = new(inputFile);
        fs.RemoveEmptySpace();
        return fs.GetChecksum();
    }

    public static long Part2()
    {
        DiskMap fs = new(inputFile);
        fs.PrintBlocks();
        fs.DefragmentFullFiles();
        return fs.GetChecksum();
    }

}


