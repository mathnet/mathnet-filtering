#region MathNet Numerics, Copyright ©2004 Christoph Ruegg, Ben Houston 

// MathNet Numerics, part of MathNet
//
// Copyright (c) 2004,	Christoph Ruegg, http://www.cdrnet.net,
// Based on Exocortex.DSP, Copyright Ben Houston, http://www.exocortex.org
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

namespace MathNet.Numerics.Transformations
{
	/// <summary>
	/// <c>ComplexTransformation</c> is a generalization for
	/// integral transformations on complex numbers.
	/// </summary>
	public abstract class ComplexTransformation
	{
		private Complex[] rawData;
		/// <summary>Mapped data source for core transformations.</summary>
		protected Complex[] viewData;
		private int viewOffset, viewStep;

		/// <summary>
		/// Accepts raw complex data to be mapped to appropriate dimensions.
		/// </summary>
		protected ComplexTransformation(Complex[] data)
		{
			this.rawData = data;
			this.viewData = null;
		}

		/// <summary>
		/// Returns the current data vector.
		/// </summary>
		/// <remarks>The returned array is not a copy but refers to the internal storage.</remarks>
		public Complex[] Data
		{
			get {return rawData;}
		}

		/// <summary>
		/// The core transformation implementation for one dimension.
		/// </summary>
		/// <param name="forward">Indicates the transformation direction.</param>
		protected abstract void TransformCore(bool forward);

		#region Transformation Interface
		/// <summary>
		/// Forward Transformation in one dimension.
		/// </summary>
		public void TransformForward1D()
		{
			for(int x=0;x<rawData.Length;x++)
				if((x&1) != 0)
					rawData[x] *= -1;
			Transform1D(true);
			ScaleRawData();
		}
		/// <summary>
		/// Backward Transformation in one dimension.
		/// </summary>
		public void TransformBackward1D()
		{
			Transform1D(false);
			for(int x=0;x<rawData.Length;x++)
				if((x&1) != 0)
					rawData[x] *= -1;
			ScaleRawData();
		}
		/// <summary>
		/// Forward Transformation in two dimensions.
		/// </summary>
		public void TransformForward2D(int width)
		{
			int height = rawData.Length / width;
			int offset = 0;
			for(int y=0;y<height;y++) 
				for(int x=0;x<width;x++) 
				{
					if(((x+y)&1) != 0)
						rawData[offset] *= -1;
					offset++;
				}
			Transform2D(true,width);
			ScaleRawData();
		}
		/// <summary>
		/// Backward Transformation in two dimensions.
		/// </summary>
		public void TransformBackward2D(int width)
		{
			Transform2D(false,width);
			int height = rawData.Length / width;
			int offset = 0;
			for(int y=0;y<height;y++) 
				for(int x=0;x<width;x++) 
				{
					if(((x+y)&1) != 0)
						rawData[offset] *= -1;
					offset++;
				}
			ScaleRawData();
		}
		/// <summary>
		/// Forward Transformation in three dimensions.
		/// </summary>
		public void TransformForward3D(int width, int height)
		{
			int depth = rawData.Length / (width*height);
			int offset = 0;
			for(int z=0;z<depth;z++)
				for(int y=0;y<height;y++) 
					for(int x=0;x<width;x++) 
					{
						if(((x+y+z)&1) != 0)
							rawData[offset] *= -1;
						offset++;
					}
			Transform3D(true,width,height);
			ScaleRawData();
		}
		/// <summary>
		/// Backward Transformation in three dimensions.
		/// </summary>
		public void TransformBackward3D(int width, int height)
		{
			Transform3D(false,width,height);
			int depth = rawData.Length / (width*height);
			int offset = 0;
			for(int z=0;z<depth;z++)
				for(int y=0;y<height;y++) 
					for(int x=0;x<width;x++) 
					{
						if(((x+y+z)&1) != 0)
							rawData[offset] *= -1;
						offset++;
					}
			ScaleRawData();
		}
		#endregion

		#region Transformation Implementations
		private void Transform1D(bool forward)
		{
			viewData = rawData;
			TransformCore(forward);
		}
		private void Transform2D(bool forward, int width)
		{
			int height = rawData.Length / width;
			if(width > 1)
				for(int y=0;y<height;y++)
				{
					LockViewData(y*width,1,width);
					TransformCore(forward);
					UnlockViewData();
				}
			if(height > 1)
				for(int x=0;x<width;x++)
				{
					LockViewData(x,width,height);
					TransformCore(forward);
					UnlockViewData();
				}
		}
		private void Transform3D(bool forward, int width, int height)
		{
			int area = width*height;
			int depth = rawData.Length / area;

			if(width > 1)
				for(int z=0;z<depth;z++)
					for(int y=0;y<height;y++)
					{
						LockViewData(y*width+z*area,1,width);
						TransformCore(forward);
						UnlockViewData();
					}
			if(height > 1)
				for(int z=0;z<depth;z++)
					for(int x=0;x<width;x++)
					{
						LockViewData(x+z*area,width,height);
						TransformCore(forward);
						UnlockViewData();
					}
			if(depth > 1)
				for(int y=0;y<depth;y++)
					for(int x=0;x<width;x++)
					{
						LockViewData(x+y*width,area,depth);
						TransformCore(forward);
						UnlockViewData();
					}
		}
		#endregion

		#region View Data locking and manipulation
		private void LockViewData(int offset, int step, int length)
		{
			viewOffset = offset;
			viewStep = step;

			if(viewData == null || viewData.Length != length)
			{
				if(offset == 0 && step == 1 && length == rawData.Length)
				{
					viewData = rawData;
					return;
				}
				viewData = new Complex[length];
			}

			int j = offset;
			for(int i=0;i<length;i++)
			{
				viewData[i] = rawData[j];
				j += step;
			}
		}

		private void UnlockViewData()
		{
			if(viewData == rawData)
				return;
			int j = viewOffset;
			for(int i=0;i<viewData.Length;i++)
			{
				rawData[j] = viewData[i];
				j += viewStep;
			}
		}

		private void ScaleRawData()
		{
			double scale = 1d/Math.Sqrt(rawData.Length);
			for(int i=0;i<rawData.Length;i++)
				rawData[i] = rawData[i]*scale;
		}

		/// <summary>
		/// Swaps two elements in the mapped data array by indexes.
		/// </summary>
		protected void SwapViewData(int first, int second)
		{
			Complex tmp = viewData[first];
			viewData[first] = viewData[second];
			viewData[second] = tmp;
		}
		#endregion
	}

	/// <summary>
	/// <c>RealTransformation</c> is a generalization for
	/// integral transformations on real numbers.
	/// </summary>
	public abstract class RealTransformation
	{
		private double[] rawData;
		/// <summary>Mapped data source for core transformations.</summary>
		protected double[] viewData;
		private int viewOffset, viewStep;

		/// <summary>
		/// Accepts raw complex data to be mapped to appropriate dimensions.
		/// </summary>
		protected RealTransformation(double[] data)
		{
			this.rawData = data;
			this.viewData = null;
		}

		/// <summary>
		/// Returns the current data vector.
		/// </summary>
		/// <remarks>The returned array is not a copy but refers to the internal storage.</remarks>
		public double[] Data
		{
			get {return rawData;}
		}

		/// <summary>
		/// The core transformation implementation for one dimension.
		/// </summary>
		/// <param name="forward">Indicates the transformation direction.</param>
		protected abstract void TransformCore(bool forward);

		#region Transformation Interface
		/// <summary>
		/// Forward Transformation in one dimension.
		/// </summary>
		public void TransformForward1D()
		{
			for(int x=0;x<rawData.Length;x++)
				if((x&1) != 0)
					rawData[x] *= -1;
			Transform1D(true);
			ScaleRawData();
		}
		/// <summary>
		/// Backward Transformation in one dimension.
		/// </summary>
		public void TransformBackward1D()
		{
			Transform1D(false);
			for(int x=0;x<rawData.Length;x++)
				if((x&1) != 0)
					rawData[x] *= -1;
			ScaleRawData();
		}
		/// <summary>
		/// Forward Transformation in two dimensions.
		/// </summary>
		public void TransformForward2D(int width)
		{
			int height = rawData.Length / width;
			int offset = 0;
			for(int y=0;y<height;y++) 
				for(int x=0;x<width;x++) 
				{
					if(((x+y)&1) != 0)
						rawData[offset] *= -1;
					offset++;
				}
			Transform2D(true,width);
			ScaleRawData();
		}
		/// <summary>
		/// Backward Transformation in two dimensions.
		/// </summary>
		public void TransformBackward2D(int width)
		{
			Transform2D(false,width);
			int height = rawData.Length / width;
			int offset = 0;
			for(int y=0;y<height;y++) 
				for(int x=0;x<width;x++) 
				{
					if(((x+y)&1) != 0)
						rawData[offset] *= -1;
					offset++;
				}
			ScaleRawData();
		}
		/// <summary>
		/// Forward Transformation in three dimensions.
		/// </summary>
		public void TransformForward3D(int width, int height)
		{
			int depth = rawData.Length / (width*height);
			int offset = 0;
			for(int z=0;z<depth;z++)
				for(int y=0;y<height;y++) 
					for(int x=0;x<width;x++) 
					{
						if(((x+y+z)&1) != 0)
							rawData[offset] *= -1;
						offset++;
					}
			Transform3D(true,width,height);
			ScaleRawData();
		}
		/// <summary>
		/// Backward Transformation in three dimensions.
		/// </summary>
		public void TransformBackward3D(int width, int height)
		{
			Transform3D(false,width,height);
			int depth = rawData.Length / (width*height);
			int offset = 0;
			for(int z=0;z<depth;z++)
				for(int y=0;y<height;y++) 
					for(int x=0;x<width;x++) 
					{
						if(((x+y+z)&1) != 0)
							rawData[offset] *= -1;
						offset++;
					}
			ScaleRawData();
		}
		#endregion

		#region Transformation Implementations
		private void Transform1D(bool forward)
		{
			viewData = rawData;
			TransformCore(forward);
		}
		private void Transform2D(bool forward, int width)
		{
			int height = rawData.Length / width;
			if(width > 1)
				for(int y=0;y<height;y++)
				{
					LockViewData(y*width,1,width);
					TransformCore(forward);
					UnlockViewData();
				}
			if(height > 1)
				for(int x=0;x<width;x++)
				{
					LockViewData(x,width,height);
					TransformCore(forward);
					UnlockViewData();
				}
		}
		private void Transform3D(bool forward, int width, int height)
		{
			int area = width*height;
			int depth = rawData.Length / area;

			if(width > 1)
				for(int z=0;z<depth;z++)
					for(int y=0;y<height;y++)
					{
						LockViewData(y*width+z*area,1,width);
						TransformCore(forward);
						UnlockViewData();
					}
			if(height > 1)
				for(int z=0;z<depth;z++)
					for(int x=0;x<width;x++)
					{
						LockViewData(x+z*area,width,height);
						TransformCore(forward);
						UnlockViewData();
					}
			if(depth > 1)
				for(int y=0;y<depth;y++)
					for(int x=0;x<width;x++)
					{
						LockViewData(x+y*width,area,depth);
						TransformCore(forward);
						UnlockViewData();
					}
		}
		#endregion

		#region View Data locking and manipulation
		private void LockViewData(int offset, int step, int length)
		{
			viewOffset = offset;
			viewStep = step;

			if(viewData == null || viewData.Length != length)
			{
				if(offset == 0 && step == 1 && length == rawData.Length)
				{
					viewData = rawData;
					return;
				}
				viewData = new double[length];
			}

			int j = offset;
			for(int i=0;i<length;i++)
			{
				viewData[i] = rawData[j];
				j += step;
			}
		}

		private void UnlockViewData()
		{
			if(viewData == rawData)
				return;

			int j = viewOffset;
			for(int i=0;i<viewData.Length;i++)
			{
				rawData[j] = viewData[i];
				j += viewStep;
			}
		}

		private void ScaleRawData()
		{
			double scale = 1d/Math.Sqrt(rawData.Length);
			for(int i=0;i<rawData.Length;i++)
				rawData[i] = rawData[i]*scale;
		}

		/// <summary>
		/// Swaps two elements in the mapped data array by indexes.
		/// </summary>
		protected void SwapViewData(int first, int second)
		{
			double tmp = viewData[first];
			viewData[first] = viewData[second];
			viewData[second] = tmp;
		}
		#endregion
	}
}
