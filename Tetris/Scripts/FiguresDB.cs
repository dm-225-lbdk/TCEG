using System.Collections.Generic;

namespace Game
{
    public class TetrisDB
    {
        private List<List<List<int>>> FiguresList { get; } = new(){
            new(){
                new(){0,1,1},
                new(){1,1,0}
            },
            new(){
                new(){2,2},
                new(){2,2}
            },
            new(){
                new(){3},
                new(){3},
                new(){3},
                new(){3}
            },
            new(){
                new(){0,4},
                new(){0,4},
                new(){4,4}
            },
            new(){
                new(){5,0},
                new(){5,0},
                new(){5,5}
            },
            new(){
                new(){6,6,0},
                new(){0,6,6}
            },
            new(){
                new(){0,7,0},
                new(){7,7,7}
            }
        };
        public IReadOnlyList<List<List<int>>> Figures => FiguresList;


        

    }
}