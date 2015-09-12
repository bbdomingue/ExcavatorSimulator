#include "StdAfx.h"

#include "Force Field.h"
#include "Groups Mask.h"
#include "Functions.h"
#include "Math.h"
#include "Scene.h"
#include "Force Field Description.h"
#include "Force Field Shape.h"
#include "Force Field Shape Description.h"
#include "Box Force Field Shape.h"
#include "Sphere Force Field Shape.h"
#include "Capsule Force Field Shape.h"
#include "Actor.h"
#include "Element Collection.h"
#include "Convex Force Field Shape.h"
#include "Convex Force Field Shape Description.h"
#include "Force Field Shape Group.h"

#include <NxForceField.h> 
#include <NxForceFieldShape.h> 

using namespace StillDesign::PhysX;

ForceField::SamplePointForcesResult::SamplePointForcesResult( array<Vector3>^ forces, array<Vector3>^ torques )
{
	_forces = forces;
	_torques = torques;
}

array<Vector3>^ ForceField::SamplePointForcesResult::Forces::get()
{
	return _forces;
}
array<Vector3>^ ForceField::SamplePointForcesResult::Torques::get()
{
	return _torques;
}

//

ForceField::ForceField( NxForceField* forceField )
{
	Debug::Assert( forceField != NULL );
	
	ObjectTable::Add( (intptr_t)forceField, this );
	
	_forceField = forceField;
	_scene = ObjectTable::GetObject<StillDesign::PhysX::Scene^>( (intptr_t)(&forceField->getScene()) );
	
	if( forceField->getActor() != NULL )
		_actor = ObjectTable::GetObject<StillDesign::PhysX::Actor^>( (intptr_t)forceField->getActor() );
	_forceFieldKernel = ObjectTable::GetObject<StillDesign::PhysX::ForceFieldKernel^>( (intptr_t)forceField->getForceFieldKernel() );
	
	_shapeGroups = gcnew ElementCollection< ForceFieldShapeGroup^ >();
	forceField->resetShapeGroupsIterator();
	for( unsigned int x = 0; x < forceField->getNbShapeGroups(); x++ )
	{
		_shapeGroups->Add( ObjectTable::GetObject<ForceFieldShapeGroup^>( (intptr_t)forceField->getNextShapeGroup() ) );
	}
}
ForceField::~ForceField()
{
	this->!ForceField();
}
ForceField::!ForceField()
{
	if( this->IsDisposed )
		return;
	
	OnDisposing( this, nullptr );
	
	_scene->UnmanagedPointer->releaseForceField( *_forceField );
	
	_forceField = NULL;
	
	_scene = nullptr;
	_actor = nullptr;
	_shapeGroups = nullptr;
	_userData = nullptr;
	
	OnDisposed( this, nullptr );
}
bool ForceField::IsDisposed::get()
{
	return ( _forceField == NULL );
}

void ForceField::AddShapeGroup( ForceFieldShapeGroup^ group )
{
	ThrowIfNullOrDisposed( group, "group" );
	
	_shapeGroups->Add( group );
	
	_forceField->addShapeGroup( *group->UnmanagedPointer );
}
bool ForceField::RemoveShapeGroup( ForceFieldShapeGroup^ group )
{
	ThrowIfNullOrDisposed( group, "group" );
	
	_forceField->removeShapeGroup( *group->UnmanagedPointer );
	
	return _shapeGroups->Remove( group );
}

ForceField::SamplePointForcesResult^ ForceField::SamplePointForces( array<Vector3>^ points )
{
	return ForceField::SamplePointForces( points, nullptr );
}
ForceField::SamplePointForcesResult^ ForceField::SamplePointForces( array<Vector3>^ points, array<Vector3>^ velocities )
{
	ThrowIfNull( points, "points" );
	
	// Check input for validity
	// If the velocities argument is supplied, it must be the same length as the points argument
	if( velocities != nullptr )
	{
		if( velocities->Length != points->Length )
			throw gcnew ArgumentException( "Invalid length. Both points and velocities must be the same length", "velocities" );
	}
	
	int numberOfPoints = points->Length;
	
	// If the number of points request is 0, then early out and return an empty structure
	if( numberOfPoints == 0 )
		return gcnew SamplePointForcesResult( gcnew array<Vector3>( 0 ), gcnew array<Vector3>( 0 ) );
	
	// Copy points and vertices into unmanaged memory
	NxVec3* p = new NxVec3[ numberOfPoints ];
	{
		pin_ptr<Vector3> b = &points[ 0 ];
		memcpy_s( p, sizeof( Vector3 ) * numberOfPoints, b, numberOfPoints * sizeof( Vector3 ) );
	}
	
	NxVec3* v;
	if( velocities != nullptr )
	{
		v = new NxVec3[ numberOfPoints ];
		{
			pin_ptr<Vector3> b = &velocities[ 0 ];
			memcpy_s( v, sizeof( Vector3 ) * numberOfPoints, b, numberOfPoints * sizeof( Vector3 ) );
		}
	}else{
		v = NULL;
	}
	
	NxVec3* forces = new NxVec3[ numberOfPoints ];
	NxVec3* torques = new NxVec3[ numberOfPoints ];
	
	//
	
	_forceField->samplePoints( numberOfPoints, p, v, forces, torques );
	
	//
	
	
	// Copy unmanaged memory into managed memory
	array<Vector3>^ f = gcnew array<Vector3>( numberOfPoints );
	array<Vector3>^ t = gcnew array<Vector3>( numberOfPoints );
	
	{
		pin_ptr<Vector3> b = &f[ 0 ];
		memcpy_s( b, sizeof( Vector3 ) * numberOfPoints, forces, numberOfPoints * sizeof( Vector3 ) );
	}
	{
		pin_ptr<Vector3> b = &t[ 0 ];
		memcpy_s( b, sizeof( Vector3 ) * numberOfPoints, torques, numberOfPoints * sizeof( Vector3 ) );
	}
	
	// Dispose of the unmanaged memory
	delete[] p, v, forces, torques;
	p = v = forces = torques = NULL;
	
	return gcnew ForceField::SamplePointForcesResult( f, t );
}

String^ ForceField::Name::get()
{
	return StillDesign::PhysX::Functions::UnmanagedToManagedString( _forceField->getName() );
}
void ForceField::Name::set( String^ value )
{
	_forceField->setName( StillDesign::PhysX::Functions::ManagedToUnmanagedString( value ) );
}
StillDesign::PhysX::Scene^ ForceField::Scene::get()
{
	return _scene;
}

StillDesign::PhysX::Actor^ ForceField::Actor::get()
{
	return _actor;
}
void ForceField::Actor::set( StillDesign::PhysX::Actor^ value )
{
	_actor = value;
	
	if( value == nullptr )
		_forceField->setActor( NULL );
	else
		_forceField->setActor( _actor->UnmanagedPointer );
}
ReadOnlyList< ForceFieldShapeGroup^ >^ ForceField::ShapeGroups::get()
{
	return _shapeGroups->ReadOnlyCollection;
}

StillDesign::PhysX::ForceFieldKernel^ ForceField::ForceFieldKernel::get()
{
	return _forceFieldKernel;
}
void ForceField::ForceFieldKernel::set( StillDesign::PhysX::ForceFieldKernel^ value )
{
	ThrowIfDisposed( value, "value" );
	
	_forceFieldKernel = value;
	
	if( value == nullptr )
		_forceField->setForceFieldKernel( NULL );
	else
		_forceField->setForceFieldKernel( value->UnmanagedPointer );
}

Matrix ForceField::Pose::get()
{
	return Math::Mat34ToMatrix( &_forceField->getPose() );
}
void ForceField::Pose::set( Matrix value )
{
	_forceField->setPose( Math::MatrixToMat34( value ) );
}

ForceFieldCoordinate ForceField::Coordinates::get()
{
	return (ForceFieldCoordinate)_forceField->getCoordinates();
}
void ForceField::Coordinates::set( ForceFieldCoordinate value )
{
	_forceField->setCoordinates( (NxForceFieldCoordinates)value );
}

short ForceField::Group::get()
{
	return _forceField->getGroup();
}
void ForceField::Group::set( short value )
{
	_forceField->setGroup( value );
}
StillDesign::PhysX::GroupsMask ForceField::GroupsMask::get()
{
	return (StillDesign::PhysX::GroupsMask)_forceField->getGroupsMask();
}
void ForceField::GroupsMask::set( StillDesign::PhysX::GroupsMask value )
{
	_forceField->setGroupsMask( (NxGroupsMask)value );
}

ForceFieldFlag ForceField::Flags::get()
{
	return (ForceFieldFlag)_forceField->getFlags();
}
void  ForceField::Flags::set( ForceFieldFlag value )
{
	_forceField->setFlags( (NxForceFieldFlags)value );
}

Object^ ForceField::UserData::get()
{
	return _userData;
}
void ForceField::UserData::set( Object^ value )
{
	_userData = value;
}

NxForceField* ForceField::UnmanagedPointer::get()
{
	return _forceField;
}