#include "stdafx.h"


#include "Configuration.h"

namespace V3
{
	static Configuration::Configuration()
	{
		EnableObjectTracking = false;
		ThrowOnError = true;
		DetectDoubleDispose = false;

		m_Watches = gcnew System::Collections::Generic::Dictionary<Result,ResultWatchFlags>();
		Timer = System::Diagnostics::Stopwatch::StartNew();
	}
	
	Configuration::Configuration()
	{
	}
	
	bool Configuration::TryGetResultWatch( Result result, ResultWatchFlags% flags )
	{
		return m_Watches->TryGetValue( result, flags );
	}
	
	void Configuration::AddResultWatch( Result result, ResultWatchFlags flags )
	{
		if( result.IsSuccess )
			throw gcnew System::ArgumentException( "result", "Cannot set a result watch on a success result." );

		if( m_Watches->ContainsKey( result ) )
			m_Watches[result] = flags;
		else
			m_Watches->Add( result, flags );
	}
	
	void Configuration::RemoveResultWatch( Result result )
	{
		if( m_Watches->ContainsKey( result ) )
			m_Watches->Remove( result );
	}
	
	void Configuration::ClearResultWatches()
	{
		m_Watches->Clear();
	}
}