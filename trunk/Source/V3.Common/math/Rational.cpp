#include "stdafx.h"


#include "Rational.h"

using namespace System;

namespace V3
{
	Rational::Rational( int numerator, int denominator ) : Numerator( numerator ), Denominator( denominator )
	{
	}

	bool Rational::operator == ( Rational left, Rational right )
	{
		return Rational::Equals( left, right );
	}

	bool Rational::operator != ( Rational left, Rational right )
	{
		return !Rational::Equals( left, right );
	}

	int Rational::GetHashCode()
	{
		return Numerator.GetHashCode() + Denominator.GetHashCode();
	}

	bool Rational::Equals( Object^ value )
	{
		if( value == nullptr )
			return false;

		if( value->GetType() != GetType() )
			return false;

		return Equals( safe_cast<Rational>( value ) );
	}

	bool Rational::Equals( Rational value )
	{
		return ( Numerator == value.Numerator && Denominator == value.Denominator );
	}

	bool Rational::Equals( Rational% value1, Rational% value2 )
	{
		return ( value1.Numerator == value2.Numerator && value1.Denominator == value2.Denominator );
	}
}