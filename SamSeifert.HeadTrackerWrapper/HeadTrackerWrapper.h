// HeadTrackerWrapper.h

#pragma once

using namespace System;

namespace HeadTrackerWrapper {

	public ref class HeadTrackerClass
	{
		public:
			int start(int * camw, int * camh);

			int update(float * ret, int * count); // Data Points
			int update(float * ret, int * count,  // Data Points
				unsigned int * ImParams, void * Buffer); // The Image

			void end();
	};
}
