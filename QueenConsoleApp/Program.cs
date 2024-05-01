using System.Diagnostics.Metrics;

namespace QueenConsoleApp
{
    /// <summary>位置類型列舉</summary>
    public enum PositionKindEnum : short
    {
        Undefine = 0,

        None = 1,

        Dot = 2,

        Queen = 3,
    }

    /// <summary>棋格</summary>
    public class ChessSquare
    {
        public int RowIndex { get; }

        public int ColumnIndex { get; }

        /// <summary>位置類型</summary>
        public PositionKindEnum PositionKind { get; set; }

        /// <summary>建構式</summary>
        public ChessSquare(int rowIndex, int columnIndex, PositionKindEnum positionKind)
        {
            this.RowIndex = rowIndex;
            this.ColumnIndex = columnIndex;
            this.PositionKind = positionKind;
        }

        /// <summary>取得備份</summary>
        public ChessSquare GetCopy()
        {
            var item = new ChessSquare(this.RowIndex, this.ColumnIndex, this.PositionKind);
            return item;
        }

        /// <summary>列印</summary>
        public string Print()
        {
            switch (this.PositionKind)
            {
                case PositionKindEnum.None:
                    return "E";

                case PositionKindEnum.Dot:
                    return ".";

                case PositionKindEnum.Queen:
                    return "Q";

                default:
                    return "U";
            }
        }
    }

    /// <summary>棋盤</summary>
    public class ChessBoard
    {
        /// <summary>棋盤尺寸</summary>
        public int BoardSize { get; }

        /// <summary>棋格列表</summary>
        public List<ChessSquare> SquareList { get; set; }

        /// <summary>建構式</summary>
        public ChessBoard(int boardSize)
        {
            this.BoardSize = boardSize;

            this.SquareList = new List<ChessSquare>();
            for (var rowIndex = 0; rowIndex < this.BoardSize; rowIndex++)
            {
                for (var columnIndex = 0; columnIndex < this.BoardSize; columnIndex++)
                {
                    var item = new ChessSquare(rowIndex, columnIndex, PositionKindEnum.None);
                    this.SquareList.Add(item);
                }
            }
        }

        /// <summary>取得備份</summary>
        public ChessBoard GetCopy()
        {
            var item = new ChessBoard(this.BoardSize);

            item.SquareList = this.SquareList
                .Select(x => x.GetCopy())
                .ToList();

            return item;
        }

        /// <summary>可否放皇后</summary>
        public bool CanPutQueen(int inputRowIndex, int inputColumnIndex)
        {
            var isExist = this.SquareList
                .Any(x =>
                    x.RowIndex == inputRowIndex
                    && x.ColumnIndex == inputColumnIndex
                    && x.PositionKind == PositionKindEnum.None);
            return isExist;
        }

        /// <summary>放皇后</summary>
        public void PutQueen(int inputRowIndex, int inputColumnIndex)
        {
            // get
            var item = this.SquareList
                .SingleOrDefault(x =>
                    x.RowIndex == inputRowIndex
                    && x.ColumnIndex == inputColumnIndex
                    && x.PositionKind == PositionKindEnum.None);
            if (item == default)
            {
                return;
            }

            // 設定水平DOT
            this.SquareList.ForEach(x =>
            {
                if (x.RowIndex == inputRowIndex)
                {
                    x.PositionKind = PositionKindEnum.Dot;
                }
            });

            // 設定垂直DOT
            this.SquareList.ForEach(x =>
            {
                if (x.ColumnIndex == inputColumnIndex
                    && x.RowIndex > inputRowIndex)
                {
                    x.PositionKind = PositionKindEnum.Dot;
                }
            });

            // 設定左下右下DOT
            this.SquareList.ForEach(x =>
            {
                if (x.RowIndex > inputRowIndex
                    && Math.Abs(x.RowIndex - inputRowIndex) == Math.Abs(x.ColumnIndex - inputColumnIndex))
                {
                    x.PositionKind = PositionKindEnum.Dot;
                }
            });

            // put queen
            item.PositionKind = PositionKindEnum.Queen;
        }

        /// <summary>列印</summary>
        public void Print()
        {
            foreach (var squareItem in this.SquareList)
            {
                Console.Write(squareItem.Print());
                if (squareItem.ColumnIndex + 1 == this.BoardSize)
                {
                    Console.WriteLine();
                }
            }
        }

    }

    /// <summary>八皇后服務</summary>
    public class EightQueenService
    {
        /// <summary>解答棋盤列表</summary>
        public List<ChessBoard> AnswerBoardList { get; private set; }

        /// <summary>建構式</summary>
        public EightQueenService()
        {
            this.AnswerBoardList = new List<ChessBoard>();
        }

        /// <summary>求解</summary>
        public void Solve(ChessBoard board, int inputRowIndex)
        {
            // 該列的每個欄位都跑過一次
            for (int columnIndex = 0; columnIndex < board.BoardSize; columnIndex++)
            {
                // 判斷可否放皇后
                if (board.CanPutQueen(inputRowIndex, columnIndex) == false)
                {
                    // 不可，換下一個欄位
                    continue;
                }

                // 可以，放皇后

                // 紀錄當前棋盤
                var newBoard = board.GetCopy();

                // 放皇后
                newBoard.PutQueen(inputRowIndex, columnIndex);

                // 判斷是否為最後一列
                if (inputRowIndex == board.BoardSize - 1)
                {
                    // 是最後一列，代表是一個解法

                    // 儲存解法
                    this.AnswerBoardList.Add(newBoard.GetCopy());

                    return;
                }

                // 不是最後一列

                // 往下一列遞迴
                this.Solve(newBoard, inputRowIndex + 1);

            }

        }

        /// <summary>列印解</summary>
        public void PrintSolve()
        {
            foreach (var answerBoardItem in AnswerBoardList)
            {
                answerBoardItem.Print();
                Console.WriteLine("==========");
            }

            Console.WriteLine($"total count: {this.AnswerBoardList.Count}");
        }
    }

    public class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Please input board size:");
            var inputBoardSizeText = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(inputBoardSizeText))
            {
                Console.WriteLine("input error");
                return;
            }
            if (int.TryParse(inputBoardSizeText, out var inputBoardSize) == false)
            {
                Console.WriteLine("input error");
                return;
            }

            var eq = new EightQueenService();
            eq.Solve(new ChessBoard(inputBoardSize), 0);
            eq.PrintSolve();
        }

    }

}
