#include "StdAfx.h"
#include "Constraint.h"
#include "Joint.h"
#include "Scene.h"
#include "Actor.h"
#include "Serializable.h"

#include <PxJoint.h>
#include <PxRigidActor.h>

Joint::Joint(PxJoint* joint, PhysX::Scene^ owner)
{
	if (joint == NULL)
		throw gcnew ArgumentNullException("joint");
	ThrowIfNullOrDisposed(owner, "owner");

	_joint = joint;
	_scene = owner;

	// Constraint
	//PxConstraint* constraint = _joint->getConstraint();
	//if (!ObjectTable::Contains((intptr_t)constraint))
	//{
	//	PhysX::Constraint^ c = gcnew PhysX::Constraint(constraint, this);
	//}

	ObjectTable::Add((intptr_t)joint, this, owner);
}
Joint::~Joint()
{
	this->!Joint();
}
Joint::!Joint()
{
	OnDisposing(this, nullptr);

	if (this->Disposed)
		return;

	_joint->release();
	_joint = NULL;

	_scene = nullptr;

	OnDisposed(this, nullptr);
}

bool Joint::Disposed::get()
{
	return (_joint == NULL);
}

Serializable^ Joint::AsSerializable()
{
	return gcnew Serializable(_joint);
}

PhysX::Constraint^ Joint::Constraint::get()
{
	return ObjectTable::GetObject<PhysX::Constraint^>((intptr_t)_joint->getConstraint());
}

JointType Joint::Type::get()
{
	return ToManagedEnum(JointType, _joint->getType());
}

PhysX::Scene^ Joint::Scene::get()
{
	return _scene;
}

PhysX::Actor^ Joint::Actor0::get()
{
	PxRigidActor* a0 = NULL;
	PxRigidActor* a1 = NULL;
	_joint->getActors(a0, a1);

	if (a0 == NULL)
		return nullptr;

	return ObjectTable::GetObject<Actor^>((intptr_t)a0);
}
PhysX::Actor^ Joint::Actor1::get()
{
	PxRigidActor* a0 = NULL;
	PxRigidActor* a1 = NULL;
	_joint->getActors(a0, a1);

	if (a1 == NULL)
		return nullptr;

	return ObjectTable::GetObject<Actor^>((intptr_t)a1);
}

Matrix Joint::LocalPose0::get()
{
	return MathUtil::PxTransformToMatrix(&_joint->getLocalPose(PxJointActorIndex::eACTOR0));
}
void Joint::LocalPose0::set(Matrix value)
{
	_joint->setLocalPose(PxJointActorIndex::eACTOR0, MathUtil::MatrixToPxTransform(value));
}

Matrix Joint::LocalPose1::get()
{
	return MathUtil::PxTransformToMatrix(&_joint->getLocalPose(PxJointActorIndex::eACTOR1));
}
void Joint::LocalPose1::set(Matrix value)
{
	_joint->setLocalPose(PxJointActorIndex::eACTOR1, MathUtil::MatrixToPxTransform(value));
}

float Joint::BreakForce::get()
{
	float f, t;
	_joint->getBreakForce(f, t);

	return f;
}
void Joint::BreakForce::set(float value)
{
	float f, t;
	_joint->getBreakForce(f, t);

	_joint->setBreakForce(value, t);
}

float Joint::BreakForceTorque::get()
{
	float f, t;
	_joint->getBreakForce(f, t);

	return t;
}
void Joint::BreakForceTorque::set(float value)
{
	float f, t;
	_joint->getBreakForce(f, t);

	_joint->setBreakForce(f, value);
}

PhysX::ConstraintFlag Joint::ConstraintFlag::get()
{
	return ToManagedEnum(PhysX::ConstraintFlag, _joint->getConstraintFlags());
}
void Joint::ConstraintFlag::set(PhysX::ConstraintFlag value)
{
	_joint->setConstraintFlags(ToUnmanagedEnum2(PxConstraintFlags, value));
}

String^ Joint::Name::get()
{
	return Util::ToManagedString(_joint->getName());
}
void Joint::Name::set(String^ value)
{
	if (_joint->getName() != NULL)
		Marshal::FreeHGlobal(IntPtr((char*)_joint->getName()));
	
	_joint->setName(Util::ToUnmanagedString(value));
}

PxJoint* Joint::UnmanagedPointer::get()
{
	return _joint;
}