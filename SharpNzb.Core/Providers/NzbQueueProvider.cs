using System;
using System.Collections.Generic;
using System.Linq;
using SharpNzb.Core.Model.Nzb;

namespace SharpNzb.Core.Providers
{
    public class NzbQueueProvider : INzbQueueProvider
    {
        private readonly List<NzbModel> _list;
        private readonly object _lock = new object();

        public NzbQueueProvider()
        {
            _list = new List<NzbModel>();
        }

        #region IQueueProvider Members

        public bool Add(NzbModel nzb)
        {
            _list.Add(nzb);
            return true;
        }

        public bool Insert(NzbModel nzb, int index)
        {
            _list.Insert(index, nzb);
            return true;
        }

        public bool MoveDown(Guid id)
        {
            lock (_lock)
            {

                int index = _list.FindIndex(n => n.Id == id); //Get the index of the idect

                if (index < 0) //return false if the idect could not be found
                    return false;

                int length = _list.Count();

                if (index == length - 1) //return because idect is already last in Queue
                    return true;
            
                var temp = _list.GetRange(index, 1).First(); //Get the idect at the index found above
                _list.RemoveAt(index); //Remove the idect
                _list.Insert(index + 1, temp); //Insert the idect at the previous position + 1
                return true;
            }
        }

        public bool MoveToBottom(Guid id)
        {
            lock (_lock)
            {
                int index = _list.FindIndex(n => n.Id == id); //Get the index of the idect

                if (index < 0) //return false if the idect could not be found
                    return false;
            
                var temp = _list.GetRange(index, 1).First(); //Get the idect at the index found above
                _list.RemoveAt(index); //Remove the idect
                _list.Add(temp); //Add temp to the end of the list (queue)
                return true;
            }
        }

        public bool MoveToTop(Guid id)
        {
            lock (_lock)
            {
                int index = _list.FindIndex(n => n.Id == id); //Get the index of the idect

                if (index < 0) //return false if the idect could not be found
                    return false;

                var temp = _list.GetRange(index, 1).First(); //Get the idect at the index found above
                _list.RemoveAt(index); //Remove the idect
                _list.Insert(0, temp); //Add temp to the start of the list (queue)
                return true;
            }
        }

        public bool MoveUp(Guid id)
        {
            lock (_lock)
            {
                int index = _list.FindIndex(n => n.Id == id); //Get the index of the idect

                if (index < 0) //return false if the idect could not be found
                    return false;

                if (index == 0) //nzb is already first
                    return true;

                var temp = _list.GetRange(index, 1).First(); //Get the idect at the index found above
                _list.RemoveAt(index); //Remove the idect
                _list.Insert(index - 1, temp); //Add temp to the start of the list (queue)
                return true;
            }
        }

        public bool Purge()
        {
            lock (_lock)
            {
                _list.Clear();
                return true;
            }
        }

        public bool Remove(Guid id)
        {
            lock (_lock)
            {
                int index = _list.FindIndex(n => n.Id == id);

                if (index < 0) //return false if the idect could not be found
                    return false;

                _list.RemoveAt(index);
                return true;
            }
        }

        public bool Sort()
        {
            throw new NotImplementedException("Queue - Sort");
        }

        public bool Swap(Guid idOne, Guid idTwo)
        {
            lock (_lock)
            {
                var indexOne = _list.FindIndex(n => n.Id == idOne);
                var indexTwo = _list.FindIndex(n => n.Id == idTwo);

                if (indexOne < 0 || indexTwo < 0) //return if either idect is not found
                    return false;

                var tempOne = _list.GetRange(indexOne, 1).First(); //Get the first idect at the index found above
                var tempTwo = _list.GetRange(indexTwo, 1).First(); //Get the second idect at the index found above
               
                //Remove both the idects
                _list.RemoveAt(indexOne);
                _list.RemoveAt(indexTwo);

                //Add them in in the others previous place
                _list.Insert(indexTwo, tempOne);
                _list.Insert(indexOne, tempTwo);

                return true;
            }
        }

        public int Index(Guid id)
        {
            lock (_lock)
            {
                return _list.FindIndex(n => n.Id == id); //Get the index of the idect
            }
        }

        public IQueryable<NzbModel> AllItems()
        {
            return _list.AsQueryable();
        }

        public IQueryable<NzbModel> Range(int start, int count)
        {
            if (count == 0) //If Count isn't privided (or is 0), return them all
                count = _list.Count;

            return _list.GetRange(start, count).AsQueryable();
        }
        #endregion
    }
}
