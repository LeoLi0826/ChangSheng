namespace ET
{
    public enum PuzzleType
    {
        ZBlock = 1,
        SBlock = 2,
        LBlock = 3,
        JBlock = 4,
        OBlock = 5,
        IBlock = 6,
        IBlock2 = 7,
        TBlock = 8,
        TBlock2 = 9,
        TBlock3 = 10,
        TBlock4 = 11
        
    }
    //拼图
    
    public class Puzzle:Entity
    {
    
        public int[,] ZBlock = new int[4, 4]
        {
            { 1, 1, 0, 0 },
            { 0, 1, 1, 0 },
            { 0, 0, 0, 0 },
            { 0, 0, 0, 0 }
        }; 
        
        public int[,] SBlock = new int[4, 4]
        {
            { 0, 1, 1, 0 },
            { 1, 1, 0, 0 },
            { 0, 0, 0, 0 },
            { 0, 0, 0, 0 }
        };
        
        public int[,] LBlock = new int[4, 4]
        {
            { 1, 0, 0 ,0 },
            { 1, 0, 0 ,0 },
            { 1, 1, 1 ,0 },
            { 0, 0, 0, 0 }
        };
        
        public int[,] JBlock = new int[4, 4]
        {
            { 0, 0, 1, 0 },
            { 0, 0, 1, 0 },
            { 1, 1, 1, 0 },
            { 0, 0, 0, 0 }
        };
        
        public int[,] OBlock = new int[4, 4]
        {
            { 1, 1, 0, 0 },
            { 1, 1, 0, 0 },
            { 0, 0, 0, 0 },
            { 0, 0, 0, 0 }
        };
        
        public int[,] IBlock = new int[4, 4]
        {
            { 1, 1, 1, 1 },
            { 0, 0, 0, 0 },
            { 0, 0, 0, 0 },
            { 0, 0, 0, 0 }
        };
        
        public int[,] IBlock2 = new int[4, 4]
        {
            { 1, 0, 0, 0 },
            { 1, 0, 0, 0 },
            { 1, 0, 0, 0 },
            { 1, 0, 0, 0 }
        };
        
        public int[,] TBlock = new int[4, 4]
        {
            { 0, 1, 0, 0 },
            { 1, 1, 1, 0 },
            { 0, 0, 0, 0 },
            { 0, 0, 0, 0 }
        };
        
        public int[,] TBlock2 = new int[4, 4]
        {
            { 0, 1, 0, 0 },
            { 0, 1, 1, 0 },
            { 0, 1, 0, 0 },
            { 0, 0, 0, 0 }
        };
        
        public int[,] TBlock3 = new int[4, 4]
        {
            { 0, 0, 0, 0 },
            { 1, 1, 1, 0 },
            { 0, 1, 0, 0 },
            { 0, 0, 0, 0 }
        };
        
        public int[,] TBlock4 = new int[4, 4]
        {
            { 0, 1, 0, 0 },
            { 1, 1, 0, 0 },
            { 0, 1, 0, 0 },
            { 0, 0, 0, 0 }
        };
        
        
        
    }
}