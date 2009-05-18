#include "stdafx.h"

#include <d3dx9.h>

#include "Half.h"
#include "Half4.h"

using namespace System;
using namespace System::Globalization;

namespace V3
{
	Half4::Half4( Half value )
	{
		X = value;
		Y = value;
		Z = value;
		W = value;
	}

	Half4::Half4( Half x, Half y, Half z, Half w )
	{
		X = x;
		Y = y;
		Z = z;
		W = w;
	}

	bool Half4::operator == ( Half4 left, Half4 right )
	{
		return Half4::Equals( left, right );
	}

	bool Half4::operator != ( Half4 left, Half4 right )
	{
		return !Half4::Equals( left, right );
	}

	int Half4::GetHashCode()
	{
		return X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode()
			 + W.GetHashCode();
	}

	bool Half4::Equals( Object^ value )
	{
		if( value == nullptr )
			return false;

		if( value->GetType() != GetType() )
			return false;

		return Equals( safe_cast<Half4>( value ) );
	}

	bool Half4::Equals( Half4 value )
	{
		return ( X == value.X && Y == value.Y && Z == value.Z
			 && W == value.W );
	}

	bool Half4::Equals( Half4% value1, Half4% value2 )
	{
		return ( value1.X == value2.X && value1.Y == value2.Y && value1.Z == value2.Z
			 && value1.W == value2.W );
	}
}