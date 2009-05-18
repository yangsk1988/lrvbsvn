#include "stdafx.h"


#include "Half.h"
#include "Half2.h"

using namespace System;
using namespace System::Globalization;

namespace V3
{
	Half2::Half2( Half value )
	{
		X = value;
		Y = value;
	}

	Half2::Half2( Half x, Half y )
	{
		X = x;
		Y = y;
	}

	bool Half2::operator == ( Half2 left, Half2 right )
	{
		return Half2::Equals( left, right );
	}

	bool Half2::operator != ( Half2 left, Half2 right )
	{
		return !Half2::Equals( left, right );
	}

	int Half2::GetHashCode()
	{
		return X.GetHashCode() + Y.GetHashCode();
	}

	bool Half2::Equals( Object^ value )
	{
		if( value == nullptr )
			return false;

		if( value->GetType() != GetType() )
			return false;

		return Equals( safe_cast<Half2>( value ) );
	}

	bool Half2::Equals( Half2 value )
	{
		return ( X == value.X && Y == value.Y );
	}

	bool Half2::Equals( Half2% value1, Half2% value2 )
	{
		return ( value1.X == value2.X && value1.Y == value2.Y );
	}
}