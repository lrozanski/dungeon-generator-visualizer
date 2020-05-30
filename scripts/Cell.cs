namespace dungeonvisualizer.scripts {
    public readonly struct Cell {

        public readonly int x;
        public readonly int y;
        public readonly string type;
        public readonly bool secret;

        public Cell(int x, int y, string type, bool secret) {
            this.x = x;
            this.y = y;
            this.type = type;
            this.secret = secret;
        }
    }
}
