#region Math.NET Iridium (LGPL) by Ruegg
// Math.NET Iridium, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2007, Christoph Rüegg,  http://christoph.ruegg.name
//						
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published 
// by the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public 
// License along with this program; if not, write to the Free Software
// Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
#endregion

using System;
using System.Collections;
using MathNet.Numerics.Properties;

namespace MathNet.Numerics
{
	/// <summary>
	/// SampleList is a sorted list in ascending order for discrete function samples x=f(t) or value pairs (t,x).
	/// Multiple values x for the same t are replaced with the mean.
	/// </summary>
	/// <remarks>This list is designed for rare additions and removals only and is slow when adding items in not-ascending order. Better fill it directly in the constructor.</remarks>
	[Serializable]
	public class SampleList : ICloneable, ICollection , IDictionary, IEnumerable
	{
		private int size;
		private int[] sampleCount; //for mean calculation
		private double[] sampleT;
		private double[] sampleX;

		private SampleList.KeyList keyList;
		private SampleList.ValueList valueList;

		public event EventHandler<SampleAlteredEventArgs> SampleAltered;
		
		#region Construction
		public SampleList()
		{
			sampleCount = new int[16];
			sampleT = new double[16];
			sampleX = new double[16];
			//size = 0;
		}
		/// <param name="capacity">initial capacity</param>
		public SampleList(int capacity)
		{
			if(capacity < 0)
				throw new ArgumentOutOfRangeException("capacity");
			sampleCount = new int[capacity];
			sampleT = new double[capacity];
			sampleX = new double[capacity];
			//size = 0;
		}
		public SampleList(IDictionary d) : this((d != null) ? d.Count : 16)
		{
			if (d == null)
				throw new ArgumentNullException("d");
			d.Keys.CopyTo(sampleT,0);
			d.Values.CopyTo(sampleX,0);
			Array.Sort(sampleT,sampleX);
			size = d.Count;
		}
		public SampleList(IDictionary d, int capacity) : this((d != null) ? (capacity >= d.Count ? capacity : d.Count) : 16)
		{
			if (d == null)
				throw new ArgumentNullException("d");
			d.Keys.CopyTo(sampleT,0);
			d.Values.CopyTo(sampleX,0);
			Array.Sort(sampleT,sampleX);
			size = d.Count;
		}
		#endregion

		#region Items
		/// <summary>
		/// Doubles the capacity. Do not use this method directly, use <see cref="Add"/> instead.
		/// </summary>
		private void EnsureCapacity(int min)
		{
			int capacitySuggestion = (sampleCount.Length < 8) ? 16 : (sampleCount.Length << 1);
			this.Capacity = (capacitySuggestion < min) ? min : capacitySuggestion;
		}

		/// <summary>
		/// Append a sample to existing samples. Do not use this method directly, use <see cref="Add"/> instead.
		/// </summary>
		private void AppendMean(int index, double x)
		{
			double cntBefore = sampleCount[index];
			double cntAfter = sampleCount[index] = sampleCount[index]+1;
			sampleX[index] = (sampleX[index]*cntBefore + x)/cntAfter;
			if(SampleAltered != null)
				SampleAltered(this,new SampleAlteredEventArgs(sampleT[index]));
		}

		/// <summary>
		/// Insert a new sample. Do not use this method directly, use <see cref="Add"/> instead.
		/// </summary>
		private void Insert(int index, double t, double x)
		{
			if(size == sampleCount.Length)
				EnsureCapacity(size + 1);
			if(index < size)
			{
				Array.Copy(sampleCount,index,sampleCount,index+1,size-index);
				Array.Copy(sampleT,index,sampleT,index+1,size-index);
				Array.Copy(sampleX,index,sampleX,index+1,size-index);
			}
			sampleCount[index] = 1;
			sampleT[index] = t;
			sampleX[index] = x;
			size++;
			if(SampleAltered != null)
				SampleAltered(this,new SampleAlteredEventArgs(t));
		}

		/// <summary>
		/// Add a new sample to the list. 
		/// </summary>
		/// <param name="t">key t, where x=f(t) or (t,x).</param>
		/// <param name="x">value x, where x=f(t) or (t,x).</param>
		public virtual void Add(double t, double x)
		{
			int index = Locate(t);
			if(index > 0 && sampleT[index] == t)
				AppendMean(index,x);
			else if(index < sampleT.Length-1 && sampleT[index+1] == t)
				AppendMean(index+1,x);
			else
				Insert(index+1,t,x);
		}

		/// <summary>
		/// Remove the sample at the given index.
		/// </summary>
		public virtual void RemoveAt(int index)
		{
			double t = sampleT[index];
			if(index < 0 || index >= size)
				throw new ArgumentOutOfRangeException("index");
			size--;
			if(index < size)
			{
				Array.Copy(sampleCount,index+1,sampleCount,index,size-index);
				Array.Copy(sampleT,index+1,sampleT,index,size-index);
				Array.Copy(sampleX,index+1,sampleX,index,size-index);
			}
			sampleCount[size] = 0;
			if(SampleAltered != null)
				SampleAltered(this,new SampleAlteredEventArgs(t));
		}

		/// <summary>
		/// Remove all samples with a key exactly equal to t, where x=f(t) or (t,x).
		/// </summary>
		public virtual void Remove(double t)
		{
			int index = IndexOfT(t);
			if(index >= 0)
				RemoveAt(index);
		}

		/// <summary>
		/// Remove all samples from the list.
		/// </summary>
		public virtual void Clear()
		{
			size = 0;
			if(SampleAltered != null)
				SampleAltered(this,new SampleAlteredEventArgs(double.NaN));
		}

		/// <summary>
		/// The index of the sample with a key exactly equal to t.
		/// </summary>
		private int IndexOfT(double t)
		{
			int index = Locate(t);
			if(sampleT[index] == t)
				return index;
			if(sampleT[index+1] == t)
				return index+1;
			return -1;
		}

		/// <summary>
		/// The index of the first sample with a value exactly equal to x.
		/// </summary>
		private int IndexOfX(double x)
		{
			return Array.IndexOf(sampleX,x,0,size);
		}

		/// <summary>
		/// Sets the X matching the given T (and sets its count to 1) or inserts a new sample.
		/// Do not use this to add a sample to the list!
		/// </summary>
		private void SetX(double t, double x)
		{
			int index = Locate(t);
			if(sampleT[index] == t)
			{
				sampleX[index] = x;
				sampleCount[index] = 1;
			}
			else if(sampleT[index+1] == t)
			{
				sampleX[index+1] = x;
				sampleCount[index+1] = 1;
			}
			else
				Insert(index+1,t,x);
		}

		/// <summary>
		/// The smalles key t in the sample list, where x=f(t) or (t,x).
		/// </summary>
		public double MinT
		{
			get {return sampleT[0];}
		}

		/// <summary>
		/// The biggest key t in the sample list, where x=f(t) or (t,x).
		/// </summary>
		public double MaxT
		{
			get {return sampleT[size-1];}
		}

		/// <summary>
		/// Get the key t stored at the given index, where x=f(t) or (t,x).
		/// </summary>
		public double GetT(int index)
		{
			return sampleT[index];
		}
		/// <summary>
		/// Get the value x stored at the given index, where x=f(t) or (t,x).
		/// </summary>
		public double GetX(int index)
		{
			return sampleX[index];
		}
		#endregion

		#region Search
		/// <summary>
		/// Find the index i of a sample near t such that t[i] &lt;= t &lt;= t[i+1]. 
		/// </summary>
		/// <returns>The lower bound index of the interval, -1 or Size-1 if out of bounds.</returns>
		public int Locate(double t)
		{
			return LocateBisection(t,-1,size);
		}

		/// <summary>
		/// Find the index i of a sample near t such that t[i] &lt;= t &lt;= t[i+1]. 
		/// </summary>
		/// <remarks>This method is faster if the expected index is near nearIndex (compared to the list size) but may be slower otherwise (worst case: factor 2 slower, best case: log2(n) faster).</remarks>
		/// <returns>The lower bound index of the interval, -1 or Size-1 if out of bounds.</returns>
		public int Locate(double t, int nearIndex)
		{
			if(nearIndex < 0 || nearIndex > size-1)
				return LocateBisection(t,-1,size);

			int lower = nearIndex;
			int upper;
			int increment = 1;

			if(t >= sampleT[lower]) // Hunt Up
			{
				if(lower == size-1)
					return lower;
				upper = lower+1;
				while(t >= sampleT[upper])
				{
					lower = upper;
					increment <<= 1;
					upper = lower+increment;
					if(upper > size-1)
					{
						upper = size;
						break;
					}
				}
			}
			else // Hunt Down
			{
				if(lower == 0)
					return -1;
				upper = lower--;
				while(t < sampleT[lower])
				{
					upper = lower;
					increment <<= 1;
					if(increment > upper)
					{
						lower = -1;
						break;
					}
					else
						lower = upper-increment;
				}
			}

			return LocateBisection(t,lower,upper);
		}

		/// <summary>
		/// Bisection Search Helper. Do not use this method directly, use <see cref="Locate"/> instead.
		/// </summary>
		private int LocateBisection(double t, int lowerIndex, int upperIndex)
		{
			int midpointIndex;

			while(upperIndex-lowerIndex > 1)
			{
				midpointIndex = (upperIndex+lowerIndex)>>1;
				if(t >= sampleT[midpointIndex])
					lowerIndex = midpointIndex;
				else
					upperIndex = midpointIndex;
			}
			if(size > 0 && t == sampleT[size-1])
				return size-2;
			else
				return lowerIndex;
		}
		#endregion

		#region Properties
		/// <summary>
		/// The count of unique samples supported.
		/// </summary>
		public int Capacity
		{
			get {return sampleCount.Length;}
			set
			{
				if(value != sampleCount.Length)
				{
					if(value < size)
						throw new ArgumentOutOfRangeException("value");
					if(value > 0)
					{
						int[] sampleCountNew = new int[value];
						double[] sampleTNew = new double[value];
						double[] sampleXNew = new double[value];
						if(size > 0)
						{
							Array.Copy(sampleCount,0,sampleCountNew,0,size);
							Array.Copy(sampleT,0,sampleTNew,0,size);
							Array.Copy(sampleX,0,sampleXNew,0,size);
						}
						sampleCount = sampleCountNew;
						sampleT = sampleTNew;
						sampleX = sampleXNew;
					}
					else
					{
						sampleCount = new int[16];
						sampleT = new double[16];
						sampleX = new double[16];
					}
				}
			}
		}
		#endregion

		#region ICloneable Member
		object ICloneable.Clone()
		{
			return Clone();
		}
		public virtual SampleList Clone()
		{
			SampleList list = new SampleList(size);
			Array.Copy(sampleCount,0,list.sampleCount,0,size);
			Array.Copy(sampleT,0,list.sampleT,0,size);
			Array.Copy(sampleX,0,list.sampleX,0,size);
			list.size = size;
			return list;
		}
		#endregion

		#region ICollection Member
		bool ICollection.IsSynchronized
		{
			get {return false;}
		}
		object ICollection.SyncRoot
		{
			get {return this;}
		}

		/// <summary>
		/// The count of unique samples stored.
		/// </summary>
		public int Count
		{
			get {return size;}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			if(array == null || array.Rank != 1)
				throw new ArgumentException("array", Resources.ArgumentSingleDimensionArray);
			if(index < 0 || (array.Length - index) < size)
				throw new ArgumentOutOfRangeException("index");
			for(int i=0;i<size;i++)
			{
				DictionaryEntry entry = new DictionaryEntry(sampleT[i], sampleX[i]);
				array.SetValue(entry, i+index);
			}
		}
		#endregion

		#region IDictionary Member
		bool IDictionary.IsReadOnly
		{
			get {return false;}
		}
		bool IDictionary.IsFixedSize
		{
			get {return false;}
		}

		object IDictionary.this[object key]
		{
			get
			{
				int index = this.IndexOfT(Convert.ToDouble(key));
				if (index >= 0)
					return this.sampleX[index];
				return null;
			}
			set
			{
				if (key == null)
					throw new ArgumentNullException("key");
				double dkey = Convert.ToDouble(key);
				double dvalue = Convert.ToDouble(value);
				SetX(dkey,dvalue);
			}
		}

		bool IDictionary.Contains(object key)
		{
			return ContainsT(Convert.ToDouble(key));
		}
		public bool ContainsT(double t)
		{
			return IndexOfT(t) != -1;
		}
		public bool ContainsX(double x)
		{
			return IndexOfX(x) != -1;
		}

		void IDictionary.Add(object key, object value)
		{
			Add(Convert.ToDouble(key),Convert.ToDouble(value));
		}
		void IDictionary.Remove(object key)
		{
			Remove(Convert.ToDouble(key));
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return new SampleListEnumerator(this,0,size,SampleListEnumerator.EnumerationMode.DictEntry);
		}

		ICollection IDictionary.Keys
		{
			get
			{
				if(keyList == null)
					keyList = new SampleList.KeyList(this);
				return keyList;
			}
		}

		ICollection IDictionary.Values
		{
			get
			{
				if(valueList == null)
					valueList = new SampleList.ValueList(this);
				return valueList;
			}
		}

		[Serializable]
		private class KeyList : IList, ICollection, IEnumerable
		{
			private SampleList sampleList;

			internal KeyList(SampleList sampleList)
			{
				this.sampleList = sampleList;
			}
			public virtual int Add(object key)
			{
				throw new NotSupportedException();
			}
			public virtual void Clear()
			{
				throw new NotSupportedException();
			}
			public virtual bool Contains(object key)
			{
				return sampleList.ContainsT(Convert.ToDouble(key));
			}
			public virtual void CopyTo(Array array, int index)
			{
				if(array == null || array.Rank != 1)
					throw new ArgumentException(Resources.ArgumentSingleDimensionArray, "array");
				Array.Copy(sampleList.sampleT,0,array,index,sampleList.Count);
			}
			public virtual IEnumerator GetEnumerator()
			{
				return new SampleList.SampleListEnumerator(sampleList,0,sampleList.Count,SampleList.SampleListEnumerator.EnumerationMode.Keys);
			}
			public virtual int IndexOf(object key)
			{
				return sampleList.IndexOfT(Convert.ToDouble(key));
			}
			public virtual void Insert(int index, object value)
			{
				throw new NotSupportedException();
			}
			public virtual void Remove(object key)
			{
				throw new NotSupportedException();
			}
			public virtual void RemoveAt(int index)
			{
				throw new NotSupportedException();
			}

			public virtual int Count
			{
				get {return sampleList.Count;}
			}
			public virtual bool IsFixedSize
			{
				get {return true;}
			}
			public virtual bool IsReadOnly
			{
				get {return true;}
			}
			public virtual bool IsSynchronized
			{
				get {return ((ICollection)sampleList).IsSynchronized;}
			}
			public virtual object this[int index]
			{
				get {return sampleList.sampleT[index];}
				set {throw new NotSupportedException();}
			}
			public virtual object SyncRoot
			{
				get {return ((ICollection)sampleList).SyncRoot;}
			}
		}

		[Serializable]
		private class ValueList : IList, ICollection, IEnumerable
		{
			private SampleList sampleList;

			internal ValueList(SampleList sampleList)
			{
				this.sampleList = sampleList;
			}
			public virtual int Add(object value)
			{
				throw new NotSupportedException();
			}
			public virtual void Clear()
			{
				throw new NotSupportedException();
			}
			public virtual bool Contains(object value)
			{
				return sampleList.ContainsX(Convert.ToDouble(value));
			}
			public virtual void CopyTo(Array array, int index)
			{
				if(array == null || array.Rank != 1)
					throw new ArgumentException();
				Array.Copy(sampleList.sampleX,0,array,index,sampleList.Count);
			}
			public virtual IEnumerator GetEnumerator()
			{
				return new SampleList.SampleListEnumerator(sampleList,0,sampleList.Count,SampleList.SampleListEnumerator.EnumerationMode.Values);
			}
			public virtual int IndexOf(object value)
			{
				return sampleList.IndexOfX(Convert.ToDouble(value));
			}
			public virtual void Insert(int index, object value)
			{
				throw new NotSupportedException();
			}
			public virtual void Remove(object value)
			{
				throw new NotSupportedException();
			}
			public virtual void RemoveAt(int index)
			{
				throw new NotSupportedException();
			}

			public virtual int Count
			{
				get {return sampleList.Count;}
			}
			public virtual bool IsFixedSize
			{
				get {return true;}
			}
			public virtual bool IsReadOnly
			{
				get {return true;}
			}
			public virtual bool IsSynchronized
			{
				get {return ((ICollection)sampleList).IsSynchronized;}
			}
			public virtual object this[int index]
			{
				get {return sampleList.sampleX[index];}
				set {throw new NotSupportedException();}
			}
			public virtual object SyncRoot
			{
				get {return ((ICollection)sampleList).SyncRoot;}
			}
		}
		#endregion

		#region IEnumerable Member
		IEnumerator IEnumerable.GetEnumerator()
		{
			return new SampleListEnumerator(this,0,size,SampleListEnumerator.EnumerationMode.DictEntry);
		}

		[Serializable]
		private class SampleListEnumerator : IDictionaryEnumerator, IEnumerator, ICloneable
		{
			private bool current;
			private int endIndex;
			private EnumerationMode mode;
			private int index;
			private object key;
			private SampleList sampleList;
			private int startIndex;
			private object value;

			internal enum EnumerationMode : int
			{
				Keys = 1,
				Values = 2,
				DictEntry = 3
			}

			internal SampleListEnumerator(SampleList sampleList, int index, int count, EnumerationMode mode)
			{
				this.sampleList = sampleList;
				this.index = index;
				this.startIndex = index;
				this.endIndex = index + count;
				this.mode = mode;
				//this.current = false;
			}
			public object Clone()
			{
				return base.MemberwiseClone();
			}
			public virtual bool MoveNext()
			{
				if (this.index < this.endIndex)
				{
					this.key = sampleList.sampleT[this.index];
					this.value = sampleList.sampleX[this.index];
					this.index++;
					this.current = true;
					return true;
				}
				this.key = null;
				this.value = null;
				this.current = false;
				return false;
			}
			public virtual void Reset()
			{
				this.index = this.startIndex;
				this.current = false;
				this.key = null;
				this.value = null;
			}
 
			public virtual object Current
			{
				get
				{
					if (!this.current)
						throw new InvalidOperationException();
					if (this.mode == EnumerationMode.Keys)
						return this.key;
					if (this.mode == EnumerationMode.Values)
						return this.value;
					return new DictionaryEntry(this.key, this.value);
				}
			}
			public virtual DictionaryEntry Entry
			{
				get
				{
					if (!this.current)
						throw new InvalidOperationException();
					return new DictionaryEntry(this.key, this.value);
				}
			}
			public virtual object Key
			{
				get
				{
					if (!this.current)
						throw new InvalidOperationException();
					return this.key;
				}
			}
			public virtual object Value
			{
				get
				{
					if (!this.current)
						throw new InvalidOperationException();
					return this.value;
				}
			}
		}
		#endregion

		#region Event Handler
		/// <summary>
		/// Event Argument for <see cref="SampleList.SampleAltered"/> Event.
		/// </summary>
		public class SampleAlteredEventArgs: EventArgs
		{
			double t;
			/// <summary>
			/// Instanciate new event arguments.
			/// </summary>
			/// <param name="t">The t-value of the x=f(t) or (t,x) samples.</param>
			public SampleAlteredEventArgs(double t)
			{
				this.t = t;
			}
			/// <summary>
			/// he t-value of the x=f(t) or (t,x) samples.
			/// </summary>
			public double T
			{
				get {return t;}
			}
		}
		#endregion
	}
}
