/* Based on http://wp7minesweeper.codeplex.com/ */

using System;
using System.Linq;
using System.Runtime.Serialization;

namespace GameCook.DirtSweeper.Core
{
    [DataContract]
    public class Tile
    {
        [DataMember]
        public bool Visited;
        [DataMember]
        public bool IsMine;
        [DataMember]
        public int SurroundMines;
        public bool Remove;
    }

    public enum GameStatus
    {
        InProgess,
        LOSE,
        WIN,
    }

    [DataContractAttribute]
    public class Game
    {
        [DataMember]
        public Tile[] tiles;
        public int Bones { get; set; }
        [DataMember]
        public GameStatus Status { get; set; }
        [DataMember]
        public int uncleared;
        [DataMember]
        public int SecondsElapsed { get; set; }
        [DataMember]
        public DateTime? Begin { get; set; }
        [DataMember]
        public int Width { get; set; }
        [DataMember]
        public int Height { get; set; }
        [DataMember]
        public int DifficultyLevel { get; set; }

        public Game(int width, int height, bool[] bones, int difficultyLevel)
        {
            this.DifficultyLevel = difficultyLevel;
            this.Width = width;
            this.Height = height;
            this.tiles = GenerateTiles(width, bones);
            this.Status = GameStatus.InProgess;
            this.uncleared = bones.Count(_ => !_);
        }

        public Tile this[int y, int x]
        {
            get
            {
                return GetTile(y, x);
            }
        }

        public Tile GetTile(int y, int x)
        {
            return tiles[y * Width + x];
        }

        public Tile GetTile(int index)
        {
            return tiles[index];
        }

        public void Dig(int index)
        {
            if (Status != GameStatus.InProgess)
                return;

            Dig(index / Width, index % Width);
        }

        public void Dig(int y, int x)
        {
            if (Begin == null)
                Begin = DateTime.Now;

            var tile = GetTile(y, x);

            if (tile.Visited)
                return;

            tile.Visited = true;
            if (tile.IsMine)
            {
                Status = GameStatus.LOSE;
                return;
            }

            if (--uncleared == 0)
            {
                Status = GameStatus.WIN;
                return;
            }
            if (tile.SurroundMines > 0 || tile.IsMine)
                return;

            if (y > 0)
                Dig(y - 1, x);

            if (y < Height - 1)
                Dig(y + 1, x);

            if (x > 0)
                Dig(y, x - 1);

            if (x < Width - 1)
                Dig(y, x + 1);

            if (y > 0 && x > 0)
                Dig(y - 1, x - 1);

            if (y > 0 && x < Width - 1)
                Dig(y - 1, x + 1);

            if (y < Height - 1 && x > 0)
                Dig(y + 1, x - 1);

            if (y < Height - 1 && x < Width - 1)
                Dig(y + 1, x + 1);
        }

        public static Game RandomGame(int width, int height, int boneCount, int difficultyLevel)
        {
            bool[] tiles = GenerateRandomMines(width * height, boneCount);
            return new Game(width, height, tiles, difficultyLevel);
        }

        public Tile[] GenerateTiles(int width, bool[] mines)
        {
            Random rnd = new Random();
            bool[] bones = mines.OrderBy(x => rnd.Next()).ToArray();

            int count = bones.Length;
            int height = bones.Length / width;
            var tiles = new Tile[count];
            for (int i = 0; i < tiles.Length; i++)
            {
                tiles[i] = new Tile { IsMine = bones[i] };
                if (tiles[i].IsMine)
                    Bones++;
            }
            for (int i = 0; i < tiles.Length; i++)
            {
                int x = i % width,
                    y = i / width;

                if (y > 0 && bones[i - width])
                    tiles[i].SurroundMines++;

                if (y < height - 1 && bones[i + width])
                    tiles[i].SurroundMines++;

                if (x > 0 && bones[i - 1])
                    tiles[i].SurroundMines++;

                if (x < width - 1 && bones[i + 1])
                    tiles[i].SurroundMines++;

                if (y > 0 && x > 0 && bones[i - width - 1])
                    tiles[i].SurroundMines++;

                if (y > 0 && x < width - 1 && bones[i - width + 1])
                    tiles[i].SurroundMines++;

                if (y < height - 1 && x > 0 && bones[i + width - 1])
                    tiles[i].SurroundMines++;

                if (y < height - 1 && x < width - 1 && bones[i + width + 1])
                    tiles[i].SurroundMines++;
            }

            return tiles;
        }

        static bool[] GenerateRandomMines(int totalCount, int mineCount)
        {
            if (mineCount > totalCount)
                throw new ArgumentException();

            var rnd = new Random();
            var tiles = new bool[totalCount];

            for (int i = 0; i < mineCount; i++)
                tiles[i] = true;

            for (int i = 0; i < mineCount - 1; i++)
            {
                var pos = rnd.Next(i, totalCount);
                var tmp = tiles[i];
                tiles[i] = tiles[pos];
                tiles[pos] = tmp;
            }

            return tiles;
        }
    }
}