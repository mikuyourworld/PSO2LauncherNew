using System.Collections;
using System.Collections.Generic;

namespace Leayal.Forms
{
    public class FakeControlCollection : ICollection<FakeControl>
    {
        List<FakeControl> controlList;
        System.Windows.Forms.Control officalParent;
        FakeControl stepParent;

        public int Count => this.controlList.Count;

        public bool IsReadOnly => false;

        private FakeControlCollection() { this.controlList = new List<FakeControl>(); }

        internal FakeControlCollection(System.Windows.Forms.Control parent) : this()
        {
            this.officalParent = parent;
        }

        internal FakeControlCollection(FakeControl parent) : this()
        {
            this.stepParent = parent;
        }

        private void SetParent(FakeControl fctl)
        {
            if (officalParent != null)
                fctl.Parent = officalParent;
            else if (stepParent != null)
                fctl.Parent = stepParent;
            else
                fctl.Parent = null;
        }

        private void UnsetParent(FakeControl fctl)
        {
            fctl.Parent = null;
        }

        public void Add(FakeControl control)
        {
            this.controlList.Add(control);
            this.SetParent(control);
            this.OnControlAdded(new FakeControlEventArgs(control));
        }

        public void AddRange(IEnumerable<FakeControl> controls)
        {
            this.controlList.AddRange(controls);
            foreach (FakeControl fctl in controls)
                this.OnControlAdded(new FakeControlEventArgs(fctl));
        }

        public IEnumerator<FakeControl> GetEnumerator()
        {
            return controlList.GetEnumerator();
        }

        public bool Remove(FakeControl control)
        {
            bool result = this.controlList.Remove(control);
            this.UnsetParent(control);
            this.OnControlRemoved(new FakeControlEventArgs(control));
            return result;
        }

        public FakeControl this[int index] => this.controlList[index];

        IEnumerator IEnumerable.GetEnumerator()
        {
            return controlList.GetEnumerator();
        }

        internal event FakeControlEventHandler ControlAdded;
        private void OnControlAdded(FakeControlEventArgs e)
        {
            this.ControlAdded?.Invoke(this, e);
        }

        internal event FakeControlEventHandler ControlRemoved;
        private void OnControlRemoved(FakeControlEventArgs e)
        {
            this.ControlRemoved?.Invoke(this, e);
        }

        public void Clear()
        {
            this.controlList.Clear();
        }

        public bool Contains(FakeControl item)
        {
            return this.controlList.Contains(item);
        }

        public void CopyTo(FakeControl[] array, int arrayIndex)
        {
            this.controlList.CopyTo(array, arrayIndex);
        }
    }
}
