# DDD Vocabulary Definitions

## Published Language

**In short by BTW**: language to communicate with a subdomain. It is frozen and
published outside of that domain, for example, as a set of message contracts for 
commands and events along with vocabulary definitions.

### By Eric Evans

> The translation between the models of two Bounded Contexts requires a common language.  
Use a well-documented shared language that can express the necessary domain information 
as a common medium of communication, translating as necessary into and out of that language.

*Source: Eric Evans: "Domain-Driven Design" [book](http://www.amazon.com/Domain-Driven-Design-Tackling-Complexity-Software/dp/0321125215) (pgs 375-376)*


## Shared Kernel #

**In short by BTW**: part of some subdomain that is extracted for reuze and frozen.

### By Eric Evans

*Source: Eric Evans: "Domain-Driven Design" [book](http://www.amazon.com/Domain-Driven-Design-Tackling-Complexity-Software/dp/0321125215) (pgs 354-355)*

> Subset of the domain model that teams have agreed to share. This includes the 
subset of code or of the database design associated with that part of the model.

> This explicitly shared stuff has special status, and shouldn't be changed without 
consultation with the other (dependent) teams.  The Shared Kernel cannot be changed 
as freely as other parts of the design.

> The Shared Kernel is often the Core Domain, some set of Generic Domains, or both, 
but it can be any part of the model that is needed by multiple teams.

> The goal is to reduce duplication and make integration between the two subsystems
relatively easy.
