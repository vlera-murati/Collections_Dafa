using System;
using System.Collections.Generic;
using System.Text;

namespace BookTourr.Logic
{
    public class ChangeUndoer
    {
        public static void Undo(LinkedList<Country> itinerary, ItineraryChange changeToUndo)
        {
            switch(changeToUndo.ChangeType)
            {
                case ChangeType.Append:
                    itinerary.RemoveLast();
                    break;
                case ChangeType.Insert:
                    LinkedListNode<Country> insertion = itinerary.First;
                    for(int i = 0; i < changeToUndo.Index; i++)
                    {
                        insertion = insertion.Next;
                    }
                    itinerary.Remove(insertion);
                    break;
                case ChangeType.Remove:
                    if(changeToUndo.Index == itinerary.Count)
                    {
                        itinerary.AddLast(changeToUndo.Value);
                    }
                    else
                    {
                        LinkedListNode<Country> removedBefore = itinerary.First;

                        for (int i = 0; i < changeToUndo.Index; i++)
                        {
                            removedBefore = removedBefore.Next;
                        }

                        itinerary.AddBefore(removedBefore, changeToUndo.Value);
                    }
                    break;

            }
        }
    }
}
