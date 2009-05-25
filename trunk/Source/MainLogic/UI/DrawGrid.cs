using System;
using System.Collections.Generic;
using System.Text;
using SlimDX.Direct3D9;

namespace VirtualBicycle.UI
{
    public class CellTextBox
    {
        public DrawTextBox TextBox;
        public int StartRow;
        public int EndRow;
        public int StartCol;
        public int EndCol;

        public CellTextBox(DrawTextBox textBox, int sRow, int eRow, int sCol, int eCol)
        {
            TextBox = textBox;
            StartRow = sRow;
            EndRow = eRow;
            StartCol = sCol;
            EndCol = eCol;
        }

        public CellTextBox(DrawTextBox textBox, int row, int col)
        {
            TextBox = textBox;
            StartRow = row;
            EndRow = row;
            StartCol = col;
            EndCol = col;
        }

        public void Unload()
        {

        }
    } 

    /// <summary>
    /// 绘制表格的类
    /// </summary>
    public class DrawGrid
    {
        #region Fields
        List<CellTextBox> textBoxCells;
        int nRow;
        int nCol;
        int[] rowLens;
        int[] colLens;
        int leftTopX;
        int leftTopY;
        bool[,] cellUsed;

        #endregion

        #region Constructor
        public DrawGrid(int nRow, int nCol, int[] rowLens, int[] colLens,int leftTopX,int leftTopY)
        {
            this.nRow = nRow;
            this.nCol = nCol;
            this.rowLens = rowLens;
            this.colLens = colLens;
            this.leftTopX = leftTopX;
            this.leftTopY = leftTopY;
            if ((nRow != rowLens.Length) || (nCol != colLens.Length))
            {
                throw new Exception("行数或列数数量不一致");
            }

            cellUsed = new bool[nRow, nCol];
            textBoxCells = new List<CellTextBox>();
        }
        #endregion

        #region Methods
        public bool AddTextBox(int sRow, int eRow, int sCol, int eCol, DrawTextBox textBox)
        {
            //首先检查是否这个区域被使用过
            for (int i = sRow; i <= eRow; i++)
            {
                for (int j = sCol; j <= eCol; j++)
                {
                    if (cellUsed[i, j])
                    {
                        return false;
                    }
                }
            }

            //然后设置该区域已经被使用
            for (int i = sRow; i <= eRow; i++)
            {
                for (int j = sCol; j <= eCol; j++)
                {
                    cellUsed[i, j] = true;
                }
            }

            //新建一个cellTextBox
            CellTextBox cellTB = new CellTextBox(textBox, sRow, eRow, sCol, eCol);

            //把TextBox的Rect设置正确
            int posX = 0;
            int posY = 0;
            int width = 0;
            int height = 0;

            for (int i = 0; i < sRow; i++)
            {
                posY += rowLens[i];
            }
            posY += leftTopY;

            for (int i = 0; i < sCol; i++)
            {
                posX += colLens[i];
            }
            posX += leftTopX;

            for (int i = sRow; i <= eRow; i++)
            {
                height += rowLens[i];
            }

            for (int i = sCol; i <= eCol; i++)
            {
                width += colLens[i];
            }

            cellTB.TextBox.Rect = new System.Drawing.Rectangle(posX, posY, width, height);
            textBoxCells.Add(cellTB);
            return true;
        }

        public bool AddTextBox(int row, int col, DrawTextBox textBox)
        {
            return AddTextBox(row, row, col, col, textBox);
        }

        public void Render(Sprite sprite)
        {
            for (int i = 0; i < textBoxCells.Count; i++)
            {
                if (textBoxCells[i].TextBox != null)
                {
                    textBoxCells[i].TextBox.Render(sprite, false);
                }
            }
        }

        public void Unload()
        {
            for (int i = 0; i < textBoxCells.Count; i++)
            {
                if (textBoxCells[i].TextBox != null)
                {
                    textBoxCells[i].TextBox.Unload();
                }
            }
        }
        #endregion

    }
}
