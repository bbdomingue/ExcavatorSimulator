#include "StdAfx.h"
#include "VehicleWheels.h"
#include "Physics.h"
#include "Actor.h"
#include <PxVehicleWheels.h> 

VehicleWheels::VehicleWheels(PxVehicleWheels* wheels, PhysX::Physics^ owner)
{
	if (wheels == NULL)
		throw gcnew ArgumentNullException("wheels");
	ThrowIfNullOrDisposed(owner, "owner");

	_wheels = wheels;

	ObjectTable::Add((intptr_t)wheels, this, owner);
}
VehicleWheels::~VehicleWheels()
{
	this->!VehicleWheels();
}
VehicleWheels::!VehicleWheels()
{
	OnDisposing(this, nullptr);

	if (this->Disposed)
		return;
	
	// The derived types will dispose of the wheel instance.
	_wheels = NULL;

	OnDisposed(this, nullptr);
}

bool VehicleWheels::Disposed::get()
{
	return (_wheels == NULL);
}

void VehicleWheels::SetWheelShapeMapping(int wheelId, int shapeId)
{
	return _wheels->setWheelShapeMapping(wheelId, shapeId);
}

float VehicleWheels::ComputeForwardSpeed()
{
	return _wheels->computeForwardSpeed();
}
float VehicleWheels::ComputeSidewaysSpeed()
{
	return _wheels->computeSidewaysSpeed();
}
bool VehicleWheels::IsInAir()
{
	return _wheels->isInAir();
}
bool VehicleWheels::IsInAir(int wheelId)
{
	return _wheels->isInAir(wheelId);
}

PhysX::Physics^ VehicleWheels::Physics::get()
{
	return ObjectTable::GetObject<PhysX::Physics^>((intptr_t)_wheels);
}

int VehicleWheels::VehicleType::get()
{
	return _wheels->getVehicleType();
}

PhysX::RigidDynamic^ VehicleWheels::Actor::get()
{
	PxRigidDynamic* actor = _wheels->getRigidDynamicActor();

	return ObjectTable::GetObject<RigidDynamic^>((intptr_t)actor);
}

PxVehicleWheels* VehicleWheels::UnmanagedPointer::get()
{
	return _wheels;
}