# Published Language #
*Source: Eric Evans: Domain-Driven Design [book](http://www.amazon.com/Domain-Driven-Design-Tackling-Complexity-Software/dp/0321125215)*

- The translation between the models of two Bounded Contexts requires a common language.  Use a well-documented shared language that can express the necessary domain information as a common medium of communication, translating as necessary into and out of that language.


# Shared Kernel #
*Source: Eric Evans: Domain-Driven Design [book](http://www.amazon.com/Domain-Driven-Design-Tackling-Complexity-Software/dp/0321125215)*

- Subset of the domain model that teams have agreed to share. This includes the subset of code or of the database design associated with that part of the model.
- This explicitly shared stuff has special status, and shouldn't be changed without consultation with the other (dependent) teams.  The Shared Kernel cannot be changed as freely as other parts of the design.
- The Shared Kernel is often the Core Domain, some set of Generic Domains, or both, but it can be any part of the model that is needed by multiple teams.
- The goal is to reduce duplication and make integration between the two subsystems relatively easy.

