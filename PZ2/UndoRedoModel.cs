using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZ2
{
    public class UndoRedoModel      //ako smo neki obj menjali, stavlja stari obj u mainObject, a izmenjeni u editedObject =>
                                    //ako uradimo undo, skidamo sa canvasa izmenjenu i stavljamo main
    {
        public string OperationName;    //operacija: dodavanje, izmena itd...
        public Object mainObject;
        public Object editedObject;

        public UndoRedoModel(string operationName, object mainObject, object editedObject)
        {
            OperationName = operationName;
            this.mainObject = mainObject;
            this.editedObject = editedObject;
        }
    }
}
