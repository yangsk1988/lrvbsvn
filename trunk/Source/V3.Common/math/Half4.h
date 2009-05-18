
#pragma once

#include "../design/Half4Converter.h"

using System::Runtime::InteropServices::OutAttribute;

namespace V3
{
	/// <summary>
	/// Defines a four component vector, using half precision floating point coordinates.
	/// </summary>
	/// <unmanaged>D3DXVECTOR3_16F</unmanaged>
	[System::Serializable]
	[System::Runtime::InteropServices::StructLayout( System::Runtime::InteropServices::LayoutKind::Sequential )]
	[System::ComponentModel::TypeConverter( V3::Design::Half4Converter::typeid )]
	public value class Half4 : System::IEquatable<Half4>
	{
	public:
		/// <summary>
		/// Gets or sets the X component of the vector.
		/// </summary>
		/// <value>The X component of the vector.</value>
		Half X;

		/// <summary>
		/// Gets or sets the Y component of the vector.
		/// </summary>
		/// <value>The Y component of the vector.</value>
		Half Y;

		/// <summary>
		/// Gets or sets the Z component of the vector.
		/// </summary>
		/// <value>The Z component of the vector.</value>
		Half Z;

		/// <summary>
		/// Gets or sets the W component of the vector.
		/// </summary>
		/// <value>The W component of the vector.</value>
		Half W;

		/// <summary>
		/// Initializes a new instance of the <see cref="Half4"/> structure.
		/// </summary>
		/// <param name="value">The value to set for the X, Y, Z, and W components.</param>
		Half4( Half value );

		/// <summary>
		/// Initializes a new instance of the <see cref="Half4"/> structure.
		/// </summary>
		/// <param name="x">The X component.</param>
		/// <param name="y">The Y component.</param>
		/// <param name="z">The Z component.</param>
		/// <param name="w">The W component.</param>
		Half4( Half x, Half y, Half z, Half w );

		/// <summary>
		/// Tests for equality between two objects.
		/// </summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns><c>true</c> if <paramref name="left"/> has the same value as <paramref name="right"/>; otherwise, <c>false</c>.</returns>
		static bool operator == ( Half4 left, Half4 right );

		/// <summary>
		/// Tests for inequality between two objects.
		/// </summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns><c>true</c> if <paramref name="left"/> has a different value than <paramref name="right"/>; otherwise, <c>false</c>.</returns>
		static bool operator != ( Half4 left, Half4 right );

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		virtual int GetHashCode() override;

		/// <summary>
		/// Returns a value that indicates whether the current instance is equal to a specified object. 
		/// </summary>
		/// <param name="obj">Object to make the comparison with.</param>
		/// <returns><c>true</c> if the current instance is equal to the specified object; <c>false</c> otherwise.</returns>
		virtual bool Equals( System::Object^ obj ) override;

		/// <summary>
		/// Returns a value that indicates whether the current instance is equal to the specified object. 
		/// </summary>
		/// <param name="other">Object to make the comparison with.</param>
		/// <returns><c>true</c> if the current instance is equal to the specified object; <c>false</c> otherwise.</returns>
		virtual bool Equals( Half4 other );

		/// <summary>
		/// Determines whether the specified object instances are considered equal. 
		/// </summary>
		/// <param name="value1"></param>
		/// <param name="value2"></param>
		/// <returns><c>true</c> if <paramref name="value1"/> is the same instance as <paramref name="value2"/> or 
		/// if both are <c>null</c> references or if <c>value1.Equals(value2)</c> returns <c>true</c>; otherwise, <c>false</c>.</returns>
		static bool Equals( Half4% value1, Half4% value2 );
	};
}
