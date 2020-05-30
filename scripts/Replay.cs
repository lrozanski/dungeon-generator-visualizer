using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using Godot.Collections;
using File = System.IO.File;

namespace dungeonvisualizer.scripts {
    public class Replay : TileMap {

        private readonly List<List<Cell>> _cells = new List<List<Cell>>();

        public override void _Ready() {
            var fileDialog = GetParent().GetNode<FileDialog>("FileDialog");
            GD.Print(fileDialog.Name);

            fileDialog.Popup_();
            fileDialog.ToSignal(fileDialog, "file_selected").OnCompleted(() => {
                var file = fileDialog.CurrentFile;
                var text = File.ReadAllText(file);
                var jsonParseResult = JSON.Parse(text);
                GD.Print(text.Length);
                GD.Print(jsonParseResult);

                if (!(jsonParseResult.Result is Array cellActions)) {
                    return;
                }
                _cells.Add(new List<Cell>());
                foreach (var actions in cellActions) {
                    if (!(actions is Array cellActionList)) {
                        continue;
                    }
                    foreach (var action in cellActionList) {
                        if (!(action is Dictionary cell)) {
                            continue;
                        }
                        var x = (int) (float) cell["x"];
                        var y = (int) (float) cell["y"];
                        var type = (string) cell["type"];
                        var secret = (bool) cell["secret"];

                        var newCell = new Cell(x, y, type, secret);
                        _cells[_cells.Count - 1].Add(newCell);
                    }
                }
                Task.Factory.StartNew(ProcessCells);
            });
        }

        private async void ProcessCells() {
            for (var x = 0; x < 64; x++) {
                for (var y = 0; y < 64; y++) {
                    SetCell(x, y, 2);
                }
            }
            foreach (var cellList in _cells) {
                foreach (var cell in cellList) {
                    ProcessCell(cell);
                    await Task.Delay(15);
                }
            }
        }

        private void ProcessCell(Cell cell) {
            var cellType = ParseCellType(cell);
            SetCell(cell.x, cell.y, cellType);

            if (cell.secret) { }
        }

        private static int ParseCellType(Cell cell) {
            switch (cell.type) {
                case "FLOOR":
                    return 0;
                case "WALL":
                    return 1;
                default:
                    return 2;
            }
        }
    }
}
