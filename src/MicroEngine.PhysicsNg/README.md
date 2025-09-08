# Physics

Box2D: https://box2d.org/documentation/md_simulation.html

- PhysicsWorldDefinition: Defines a physics world. Has a DefaultPhysicsWorldDefinition.
- PhysicsWorld: keeps all instances of RigidBody.
- RigidBodyDefinition: Used for defining a RigidBody.
- RigidBody: A base of all rigid bodies. Has position and center of mass. Center of mass can be set or calculated from connected Shapes.
  Has Mass. Set or calculated from connected bodies.
  Can contain Damping and AngularDamping (how to slow over time). Contains Velocity and AngularVelocity.
  Can be Disabled, so it is ignored by the physics engine.
  You can apply a force to a body. That updates its velocities and torques.
- StaticBody: A static body does not move under simulation and behaves as if it has infinite mass.
  Internally, StaticBody stores zero for the mass and the inverse mass. A static body has zero velocity.
  Static bodies do not collide with other static or kinematic bodies. It is the default rigid body type.
- KinematicBody: A kinematic body moves under simulation according to its velocity. Kinematic bodies do not respond
  to forces. A kinematic body is moved by setting its velocity. A kinematic body behaves as if it has infinite mass,
  however, it stores zero for the mass and the inverse mass. Kinematic bodies do not collide with other kinematic
  or static bodies. Generally you should use a kinematic body if you want a shape to be animated and not affected
  by forces or collisions.
- DynamicBody: A dynamic body is fully simulated and moves according to forces and torques. A dynamic body can collide
  with all body types. A dynamic body always has finite, non-zero mass.
- Shape: Defines a shape used for collision detection. AABB, Sphere, Plane... Shapes contain collision filter, that define,
  who can collide with whom.

Bodies carry shapes and moves them around in the world. Bodies are always rigid bodies. That means that two
shapes attached to the same rigid body never move relative to each other and shapes attached to the same body don't collide.

Shapes have collision geometry and density. Normally, bodies acquire their mass properties from the shapes. However,
you can override the mass properties after a body is constructed.

You usually keep references to all the bodies you create. This way you can query the body positions to update the positions
of your graphical entities. You should also keep body references so you can destroy them when you are done with them.