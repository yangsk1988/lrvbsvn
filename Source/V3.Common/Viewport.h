
#pragma once

namespace V3
{
	/// <summary>
	/// Defines a rectangular region of a render-target surface onto which 
	/// a 3D rendering is projected.
	/// </summary>
	/// <unmanaged>D3DVIEWPORT9</unmanaged>
	public value class Viewport : System::IEquatable<Viewport>
	{
		int m_X;
		int m_Y;
		int m_Width;
		int m_Height;
		float m_MinZ;
		float m_MaxZ;

	public:
		/// <summary>
		/// Gets or sets the viewport's X position.
		/// </summary>
		property int X
		{
			int get();
			void set( int value );
		}
		
		/// <summary>
		/// Gets or sets the viewport's Y position.
		/// </summary>
		property int Y
		{
			int get();
			void set( int value );
		}
		
		/// <summary>
		/// Gets or sets the viewport's width.
		/// </summary>
		property int Width
		{
			int get();
			void set( int value );
		}
		
		/// <summary>
		/// Gets or sets the viewport's height.
		/// </summary>
		property int Height
		{
			int get();
			void set( int value );
		}

		/// <summary>
		/// Gets or sets the viewport's minimum Z depth.
		/// </summary>
		property float MinZ
		{
			float get();
			void set( float value );
		}
		
		/// <summary>
		/// Gets or sets the viewport's maximum Z depth.
		/// </summary>
		property float MaxZ
		{
			float get();
			void set( float value );
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Viewport"/> structure.
		/// </summary>
		/// <param name="x">The X coordinate of the viewport.</param>
		/// <param name="y">The Y coordinate of the viewport.</param>
		/// <param name="width">The width of the viewport.</param>
		/// <param name="height">The height of the viewport.</param>
		Viewport( int x, int y, int width, int height);

		/// <summary>
		/// Initializes a new instance of the <see cref="Viewport"/> structure.
		/// </summary>
		/// <param name="x">The X coordinate of the viewport.</param>
		/// <param name="y">The Y coordinate of the viewport.</param>
		/// <param name="width">The width of the viewport.</param>
		/// <param name="height">The height of the viewport.</param>
		/// <param name="minZ">The minimum Z distance of the viewport.</param>
		/// <param name="maxZ">The maximum Z distance of the viewport.</param>
		Viewport( int x, int y, int width, int height, float minZ, float maxZ );
		
		/// <summary>
		/// Tests for equality between two viewports.
		/// </summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns><c>true</c> if <paramref name="left"/> has the same value as <paramref name="right"/>; otherwise, <c>false</c>.</returns>
		static bool operator == ( Viewport left, Viewport right );
		
		/// <summary>
		/// Tests for inequality between two viewports.
		/// </summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns><c>true</c> if <paramref name="left"/> has a different value than <paramref name="right"/>; otherwise, <c>false</c>.</returns>
		static bool operator != ( Viewport left, Viewport right );

		/// <summary>
		/// Converts the value of the viewport to its equivalent string representation.
		/// </summary>
		/// <returns>The string representation of the value of this instance.</returns>
		virtual System::String^ ToString() override;

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		virtual int GetHashCode() override;

		/// <summary>
		/// Returns a value indicating whether this instance is equal to the specified object.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns><c>true</c> if <paramref name="obj"/> has the same value as this instance; otherwise, <c>false</c>.</returns>
		virtual bool Equals( System::Object^ obj ) override;

		/// <summary>
		/// Returns a value indicating whether this instance is equal to the specified object.
		/// </summary>
		/// <param name="other">A <see cref="Viewport"/> to compare with this instance.</param>
		/// <returns><c>true</c> if <paramref name="other"/> has the same value as this instance; otherwise, <c>false</c>.</returns>
		virtual bool Equals( Viewport other );

		/// <summary>
		/// Returns a value indicating whether the two viewports are equivalent.
		/// </summary>
		/// <param name="value1">The first value to compare.</param>
		/// <param name="value2">The second value to compare.</param>
		/// <returns><c>true</c> if <paramref name="value1"/> has the same value as <paramref name="value2"/>; otherwise, <c>false</c>.</returns>
		static bool Equals( Viewport% value1, Viewport% value2 );
	};
}