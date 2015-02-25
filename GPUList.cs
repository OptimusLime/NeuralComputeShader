/* Who:     Chris Giles
 * What:    A GPU-based implementation of List<T>
 * When:    5/16/2011 @ ~4 AM
 * Where:   My house
 * Why:     I was tired of dealing with low level graphics resources
 *          This is almost completely interchangable with built in C# List<T>
 *          Only, the backend is a DirectX11 Buffer
 *          You can use the underlying buffer in any kind of shader
 *          But with a pretty C# interface for messing with its data
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

using SlimDX;
using SlimDX.Direct3D11;

using Device = SlimDX.Direct3D11.Device;
using Buffer = SlimDX.Direct3D11.Buffer;

namespace GPUTools
{
    public class GPUList<T> : IList<T>, IDisposable where T : struct
    {
        #region GPUList Members

        private DeviceContext context;
        private Buffer buffer;
        private ShaderResourceView shaderResourceView;
        private UnorderedAccessView unorderedAccessView;

        private int count;
        private int capacity;

        /// <summary>
        /// Makes a new list given initial data
        /// </summary>
        /// <param name="context">Device context on which to perform all operations</param>
        public GPUList(DeviceContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Makes a new empty list with an initial capacity
        /// </summary>
        /// <param name="context">Device context on which to perform all operations</param>
        /// <param name="initialCapacity">The initial capacity</param>
        public GPUList(DeviceContext context, int initialCapacity)
        {
            this.context = context;
            this.setCapacity(initialCapacity);
        }

        /// <summary>
        /// Makes a new list given initial data
        /// </summary>
        /// <param name="context">Device context on which to perform all operations</param>
        /// <param name="data">The initial data</param>
        public GPUList(DeviceContext context, IEnumerable<T> data)
        {
            this.context = context;
            this.AddRange(data);
        }

        public Buffer CreateStagingBuffer()
        {
            int dataSize = Marshal.SizeOf(typeof(T));
            return new Buffer(context.Device, dataSize * count, ResourceUsage.Staging,
                BindFlags.None, CpuAccessFlags.Read, ResourceOptionFlags.None, 0);
        }

        Buffer stagingBuffer = null;
        int bufferCount;
        public Buffer GetStagingBuffer()
        {
            if (stagingBuffer != null && count == bufferCount)
            {
                return stagingBuffer;
            }
            else if (stagingBuffer != null && count != bufferCount)
                DestroyStagingBuffer();

            //create a staging buffer ONLY ONCE for a given size
            stagingBuffer = CreateStagingBuffer();
            bufferCount = count;

            //send it back
            return stagingBuffer;
        }
        public void DestroyStagingBuffer()
        {
            stagingBuffer.Dispose();
            stagingBuffer = null;
        }

        /// <summary>
        /// Gets the DeviceContext on which to perform all operations
        /// </summary>
        public DeviceContext Context { get { return context; } }

        /// <summary>
        /// Gets the underlying Buffer stored on the GPU
        /// </summary>
        public Buffer GPUBuffer { get { return buffer; } }

        /// <summary>
        /// Gets a ShaderResourceView for use in a shader
        /// </summary>
        public ShaderResourceView ShaderResource { get { return shaderResourceView; } }

        /// <summary>
        /// Gets a UnorderedAccessView for use in a shader
        /// </summary>
        public UnorderedAccessView UnorderedAccess { get { return unorderedAccessView; } }

        /// <summary>
        /// Gets or Sets the internal GPU buffer capacity in number of elements
        /// </summary>
        public int Capacity { get { return capacity; } set { setCapacity(value); } }

        /// <summary>
        /// Adds a range of items to the end of the list
        /// </summary>
        /// <param name="range"></param>
        public void AddRange(IEnumerable<T> range)
        {
            if (range == null)
                throw new ArgumentNullException();

            int dataSize = Marshal.SizeOf(typeof(T));

            if (range is GPUList<T> && range != this)
            {
                GPUList<T> list = range as GPUList<T>;

                if (list.Count == 0)
                    return;
                if (Count + list.Count > Capacity)
                    setCapacity(2 * Count + list.Count);

                count += list.Count;

                list.CopyRangeTo(0, list.Count, this, Count - list.Count);
            }
            else
            {
                T[] data = null;
                if (range is T[])
                    data = (T[])range;
                else if (range is List<T>)
                    data = (range as List<T>).ToArray();
                else
                {
                    List<T> dataList = new List<T>(range);
                    data = dataList.ToArray();
                }

                if (data.Length == 0)
                    return;
                if (Count + data.Length > Capacity)
                    setCapacity(2 * Count + data.Length);

                count += data.Length;

                SetRange(Count - data.Length, data, 0, data.Length);
            }
        }

        /// <summary>
        /// Inserts a range of items into the list
        /// </summary>
        /// <param name="index">Position in the list to insert items</param>
        /// <param name="range">The range of items to insert</param>
        public void InsertRange(int index, IEnumerable<T> range)
        {
            if (range == null)
                throw new ArgumentNullException();
            if (index < 0 || index > Count)
                throw new IndexOutOfRangeException();

            if (index == Count)
            {
                AddRange(range);
            }
            else
            {
                GPUList<T> newList = null;

                if (range is GPUList<T>)
                {
                    GPUList<T> list = range as GPUList<T>;

                    if (list.Count == 0)
                        return;

                    newList = new GPUList<T>(Context, list.Count + Count);
                    newList.count = list.Count + Count;
                    this.CopyRangeTo(0, index, newList, 0);
                    list.CopyTo(newList, index);
                    this.CopyRangeTo(index, Count - index, newList, index + list.Count);
                }
                else
                {
                    T[] data = null;
                    if (range is T[])
                        data = (T[])range;
                    else if (range is List<T>)
                        data = (range as List<T>).ToArray();
                    else
                    {
                        List<T> dataList = new List<T>(range);
                        data = dataList.ToArray();
                    }

                    if (data.Length == 0)
                        return;

                    newList = new GPUList<T>(Context, data.Length + Count);
                    newList.count = data.Length + Count;
                    this.CopyRangeTo(0, index, newList, 0);
                    newList.SetRange(index, data, 0, data.Length);
                    this.CopyRangeTo(index, Count - index, newList, index + data.Length);
                }

                setCapacity(0);
                count = newList.count;
                capacity = newList.capacity;
                buffer = newList.buffer;
                shaderResourceView = newList.shaderResourceView;
                unorderedAccessView = newList.unorderedAccessView;
            }
        }

        /// <summary>
        /// Removes a range of items from the list
        /// </summary>
        /// <param name="index">Index to start removing</param>
        /// <param name="count">Number of items to remove</param>
        public void RemoveRange(int index, int count)
        {
            if (count == 0)
                return;

            if (count < 0)
                throw new IndexOutOfRangeException();
            if (index < 0 || index >= Count)
                throw new IndexOutOfRangeException();
            if (index + count < 0 || index + count > Count)
                throw new IndexOutOfRangeException();

            if (index + count == Count)
            {
                this.count -= count;
                return;
            }

            GPUList<T> newList = new GPUList<T>(Context, Capacity);
            newList.count = this.count - count;
            this.CopyRangeTo(0, index, newList, 0);
            this.CopyRangeTo(index + count, this.count - (index + count), newList, index);

            setCapacity(0);
            this.count = newList.count;
            this.capacity = newList.capacity;
            this.buffer = newList.buffer;
            this.shaderResourceView = newList.shaderResourceView;
            this.unorderedAccessView = newList.unorderedAccessView;
        }

        /// <summary>
        /// Sets a range of data in the list based on an array
        /// </summary>
        /// <param name="index">Index in the GPUList to start setting data</param>
        /// <param name="array">The array of data to use</param>
        /// <param name="arrayIndex">The start index in the array</param>
        /// <param name="count">The number of elements to set</param>
        public void SetRange(int index, T[] array, int arrayIndex, int count)
        {
            if (count == 0)
                return;

            if (count < 0)
                throw new IndexOutOfRangeException();
            if (index < 0 || index >= Count)
                throw new IndexOutOfRangeException();
            if (index + count < 0 || index + count > Count)
                throw new IndexOutOfRangeException();

            if (array == null)
                throw new ArgumentNullException();
            if (arrayIndex < 0 || arrayIndex >= array.Length)
                throw new IndexOutOfRangeException();
            if (arrayIndex + array.Length < 0 || arrayIndex + count > array.Length)
                throw new IndexOutOfRangeException();

            int dataSize = Marshal.SizeOf(typeof(T));
            DataStream stream = new DataStream(array, true, false);
            stream.Seek(arrayIndex, System.IO.SeekOrigin.Begin);
            context.UpdateSubresource(new DataBox(0, 0, stream), buffer, 0,
                new ResourceRegion(dataSize * index, 0, 0, dataSize * (index + count), 1, 1));
        }

        /// <summary>
        /// Copies a range of data to an array
        /// </summary>
        /// <param name="index">Index from which to start copying</param>
        /// <param name="count">Number of elements to copy</param>
        /// <param name="array">Destination array to copy to</param>
        /// <param name="arrayIndex">Destination array index to copy to</param>
        public void CopyRangeTo(int index, int count, T[] array, int arrayIndex)
        {
            if (count == 0)
                return;

            if (count < 0)
                throw new IndexOutOfRangeException();
            if (index < 0 || index >= Count)
                throw new IndexOutOfRangeException();
            if (index + count < 0 || index + count > Count)
                throw new IndexOutOfRangeException();

            if (array == null)
                throw new ArgumentNullException();
            if (arrayIndex < 0 || arrayIndex >= array.Length)
                throw new IndexOutOfRangeException();
            if (arrayIndex + array.Length < 0 || arrayIndex + count > array.Length)
                throw new IndexOutOfRangeException();

            int dataSize = Marshal.SizeOf(typeof(T));
            //Buffer stagingBuffer = new Buffer(context.Device, dataSize * count, ResourceUsage.Staging,
            //    BindFlags.None, CpuAccessFlags.Read, ResourceOptionFlags.None, 0);
            Buffer bufferToUse = GetStagingBuffer();


            context.CopySubresourceRegion(buffer, 0,
                new ResourceRegion(dataSize * index, 0, 0, dataSize * (index + count), 1, 1),
                bufferToUse, 0, 0, 0, 0);

            DataBox box = context.MapSubresource(bufferToUse, 
                MapMode.Read, SlimDX.Direct3D11.MapFlags.None);
            box.Data.ReadRange<T>(array, arrayIndex, count);
            context.UnmapSubresource(bufferToUse, 0);

            //stagingBuffer.Dispose();
        }

        /// <summary>
        /// Copies a range of data to another GPUList
        /// </summary>
        /// <param name="index">Index from which to start copying</param>
        /// <param name="count">Number of elements to copy</param>
        /// <param name="list">Destination list to copy to</param>
        /// <param name="listIndex">Destination list index to copy to</param>
        public void CopyRangeTo(int index, int count, GPUList<T> list, int listIndex)
        {
            if (count == 0)
                return;

            if (list == this)
                throw new Exception("Cannot copy range into self...yet");

            if (count < 0)
                throw new IndexOutOfRangeException();
            if (index < 0 || index >= Count)
                throw new IndexOutOfRangeException();
            if (index + count < 0 || index + count > Count)
                throw new IndexOutOfRangeException();

            if (list == null)
                throw new ArgumentNullException();
            if (listIndex < 0 || listIndex >= list.Count)
                throw new IndexOutOfRangeException();
            if (listIndex + list.Count < 0 || listIndex + count > list.Count)
                throw new IndexOutOfRangeException();

            int dataSize = Marshal.SizeOf(typeof(T));
            context.CopySubresourceRegion(buffer, 0,
                new ResourceRegion(dataSize * index, 0, 0, dataSize * (index + count), 1, 1),
                list.buffer, 0, dataSize * listIndex, 0, 0);
        }

        /// <summary>
        /// Copys the list to another GPUList
        /// </summary>
        /// <param name="array">The list to copy data to</param>
        /// <param name="arrayIndex">The starting index in the array</param>
        public void CopyTo(GPUList<T> list, int listIndex)
        {
            CopyRangeTo(0, count, list, listIndex);
        }

        /// <summary>
        /// Trims excess capacity on the internal GPU buffer to match the count
        /// </summary>
        public void TrimExcess()
        {
            setCapacity(Count);
        }

        /// <summary>
        /// Returns a copy of the list into an array
        /// </summary>
        /// <returns></returns>
        public T[] ToArray()
        {
            T[] data = new T[Count];
            CopyTo(data, 0);
            return data;
        }

        /// <summary>
        /// Sets the internal capacity of the list
        /// </summary>
        /// <param name="newCapacity"></param>
        private void setCapacity(int newCapacity)
        {
            if (newCapacity < 0)
                throw new ArgumentOutOfRangeException();

            if (newCapacity == capacity)
                return;

            if (newCapacity == 0)
            {
                if (unorderedAccessView != null)
                    unorderedAccessView.Dispose();
                if (shaderResourceView != null)
                    shaderResourceView.Dispose();
                if (buffer != null)
                    buffer.Dispose();

                unorderedAccessView = null;
                shaderResourceView = null;
                buffer = null;

                capacity = count = 0;
                return;
            }

            int dataSize = Marshal.SizeOf(typeof(T));
            Buffer newBuffer = new Buffer(context.Device, dataSize * newCapacity, ResourceUsage.Default,
                BindFlags.ShaderResource | BindFlags.UnorderedAccess,
                CpuAccessFlags.None, ResourceOptionFlags.StructuredBuffer, dataSize);

            int sizeToCopy = Math.Min(newCapacity, Count);
            if (sizeToCopy > 0)
                context.CopySubresourceRegion(buffer, 0, new ResourceRegion(0, 0, 0, dataSize * sizeToCopy, 1, 1), newBuffer, 0, 0, 0, 0);

            count = sizeToCopy;
            capacity = newCapacity;

            if (unorderedAccessView != null)
                unorderedAccessView.Dispose();
            if (shaderResourceView != null)
                shaderResourceView.Dispose();
            if (buffer != null)
                buffer.Dispose();

            buffer = newBuffer;
            unorderedAccessView = new UnorderedAccessView(context.Device, buffer);
            shaderResourceView = new ShaderResourceView(context.Device, buffer);
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Disposes the list by destroying all GPU resources used
        /// </summary>
        public void Dispose()
        {
            setCapacity(0);
            if (stagingBuffer != null)
                DestroyStagingBuffer();
        }

        #endregion

        #region IList<T> Members

        /// <summary>
        /// Adds one item to the end of the list
        /// </summary>
        /// <param name="item">The item to add</param>
        public void Add(T item)
        {
            T[] array = new T[] { item };
            AddRange(array);
        }

        /// <summary>
        /// Gets the index in the list of the first item found
        /// </summary>
        /// <param name="item">The item whose index is searched for</param>
        /// <returns>Index of the item found, or -1 if not found</returns>
        public int IndexOf(T item)
        {
            T[] data = new T[count];
            CopyTo(data, 0);

            for (int i = 0; i < data.Length; i++)
                if (data[i].Equals(item))
                    return i;
            return -1;
        }

        /// <summary>
        /// Inserts an item at a specific index
        /// </summary>
        /// <param name="index">The index in the list to insert the item</param>
        /// <param name="item">The item to insert</param>
        public void Insert(int index, T item)
        {
            T[] array = new T[] { item };
            InsertRange(index, array);
        }

        /// <summary>
        /// Removes an item at a specific index
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            RemoveRange(index, 1);
        }

        /// <summary>
        /// Gets or Sets an element at some index in the list
        /// </summary>
        /// <param name="index">Index in the list to get or set</param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= count)
                    throw new IndexOutOfRangeException();

                T[] array = new T[1];
                CopyRangeTo(index, 1, array, 0);
                return array[0];
            }
            set
            {
                if (index < 0 || index >= count)
                    throw new IndexOutOfRangeException();

                T[] array = new T[] { value };
                SetRange(index, array, 0, 1);
            }
        }

        #endregion

        #region ICollection<T> Members

        /// <summary>
        /// Clears the list of all elements
        /// </summary>
        public void Clear()
        {
            count = 0;
        }

        /// <summary>
        /// Returns true if the item is contained in the list, or false if not
        /// </summary>
        /// <param name="item">The item to check containment for</param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            foreach (T i in this)
                if (i.Equals(item))
                    return true;
            return false;
        }

        /// <summary>
        /// Copys the list to an array
        /// </summary>
        /// <param name="array">The array to copy data to</param>
        /// <param name="arrayIndex">The starting index in the array</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            CopyRangeTo(0, count, array, arrayIndex);
        }

        /// <summary>
        /// Gets the number of items in the list
        /// </summary>
        public int Count { get { return count; } }

        /// <summary>
        /// Returns whether or not the list is read only
        /// </summary>
        public bool IsReadOnly { get { return false; } }

        /// <summary>
        /// Removes an item from the list. Only the first item found is removed
        /// </summary>
        /// <param name="item">The item to remove</param>
        /// <returns>True if the item is removed, false otherwise</returns>
        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index >= 0)
                RemoveAt(index);
            return index >= 0;
        }

        #endregion

        #region IEnumerable<T> Members

        /// <summary>
        /// Gets the enumerator for this list. Any changes made while enumerating will not be reflected
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            T[] array = new T[count];
            CopyTo(array, 0);
            return array.AsEnumerable<T>().GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Gets the enumerator for this list. Any changes made while enumerating will not be reflected
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
