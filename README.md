# Distributed System App
## About
The main objective of this project is to represent the state-of-the-art of a **distributed**, **reliable**, and **highly scalable** system.

**Scalability** and **Resilience** require **low coupling** and **high cohesion**, principles strongly linked to the proper understanding of the business through **well-defined boundaries**, combined with a healthy and
efficient integration strategy such as **Event-driven Architecture** (EDA).

**Independence**, as the main characteristic of a **Microservice**, can only be found in a **Bounded Context**.

The [**Event Sourcing**](https://www.eventstore.com/event-sourcing) is a proprietary implementation that, in addition to being **naturally auditable** and **data-driven**, represents the most efficient persistence mechanism ever. An **eventual state
transition** Aggregate design is essential at this point. The **Event Store** comprises EF Core (ORM) + MSSQL (Database).

[**Projections**](https://www.eventstore.com/event-sourcing#Projections) are **asynchronously denormalized** and stored on a NoSQL Database(MongoDB); Nested documents should be avoided here; Each projection has its index and **fits perfectly into a view or component**,
mitigating unnecessary data traffic and making the reading side as efficient as possible.

The splitting between **Command** and **Query** stacks occurs logically through the [**CQRS**](https://cqrs.files.wordpress.com/2010/11/cqrs_documents.pdf) pattern and fiscally via a **Microservices** architecture. Each stack is an individual deployable unit with its database,
and the data flows from Command to Query stack via **Domain** and/or **Summary** events.

As a domain-centric approach, Clean Architecture provides the appropriate isolation between the Core (Application + Domain) and "times many" Infrastructure concerns.

## Installation
Run docker-compose to build Infrastructure such as Redis, MSSQL Server, RabbitMQ, Seq for development environment.
> docker compose -f docker-compose.Development.Infrastructure.yaml up --detach

## The Solution Architecture
### V1
![](https://github.com/mnnam1302/DistributedSystem_Yarp_ReverseProxy/blob/dev/.assets/imgs/solution_architecture01.png)

## Reasearch

### Domain Driven Design
> Domain-Driven Design is an approach to software development that centers the development on programming a domain model that has a rich understanding of the processes and rules of a domain. The name
> comes from a 2003 book by Eric Evans that describes the approach through a catalog of patterns. Since then a community of practitioners have further developed the ideas, spawning various other books
> and training courses. The approach is particularly suited to complex domains, where a lot of often-messy logic needs to be organized.
>
> [Fowler, Martin. "DomainDrivenDesign", _martinfowler.com_, last edited on 22 April 2020](https://martinfowler.com/bliki/DomainDrivenDesign.html)

#### Bounded Context

> Basically, the idea behind bounded context is to put a clear delineation between one model and another model. This delineation and boundary that's put around a domain model, makes the model that is
> inside the boundary very explicit with very clear meaning as to the concepts, the elements of the model, and the way that the team, including domain experts, think about the model.
>
> You'll find a ubiquitous language that is spoken by the team and that is modeled in software by the team. In scenarios and discussions where somebody says, for example, "product," they know in that
> context exactly what product means. In another context, product can have a different meaning, one that was defined by another team. The product may share identities across bounded contexts, but,
> generally speaking, the product in another context has at least a slightly different meaning, and possibly even a vastly different meaning.
>
> [Vernon, Vaughn. "Modeling Uncertainty with Reactive DDD", _www.infoq.com_, last edited on 29 Set 2018](https://martinfowler.com/bliki/BoundedContext.html)

![](https://raw.githubusercontent.com/AntonioFalcaoJr/EventualShop/release/.assets/img/BoundedContext.jpg)  
[Fig. 5: Martin, Fowler. _BoundedContext_](https://martinfowler.com/bliki/BoundedContext.html)

> First, a Bounded Context is a semantic contextual boundary. This means that within the boundary each component of the software model has a specific meaning and does specific things. The components
> inside a Bounded Context are context specific and semantically motivated. That’s simple enough.
>
> When you are just getting started in your software modeling efforts, your Bounded Context is somewhat conceptual. You could think of it as part of your problem space. However, as your model starts
> to take on deeper meaning and clarity, your Bounded Context will quickly transition to your solution space , with your software model being reflected as project source code. Remember that a Bounded
> Context is where a model is implemented, and you will have separate software artifacts for each Bounded Context.
>
> Vernon, V. (2016). "Strategic Design with Bounded Contexts and the Ubiquitous Language", Domain-Driven Design Distilled, 1st ed. New York: Addison-Wesley Professional.

> Explicitly define the context within which a model applies. Explicitly set boundaries in terms of team organization, usage within specific parts of the application, and physical manifestations such
> as code bases and database schemas. Apply Continuous Integration to keep model concepts and terms strictly consistent within these bounds, but don’t be distracted or confused by issues outside.
> Standardize a single development process within the context, which need not be used elsewhere.
>
> [Evans, Eric. (2015). "Bounded Context", Domain-Driven Design Reference](https://www.domainlanguage.com/ddd/reference)

#### Aggregate

> I think a model is a set of related concepts that can be applied to solve a problem.
> -- <cite> Eric Evans </cite>

![](https://raw.githubusercontent.com/AntonioFalcaoJr/EventualShop/release/.assets/img/aggregate.png)  
Fig. 6: Vernon, V. (2016), Aggregates from Domain-Driven Design Distilled, 1st ed, p78.

> Each Aggregate forms a transactional consistency boundary. This means that within a single Aggregate, all composed parts must be consistent, according to business rules, when the controlling
> transaction is committed to the database. This doesn't necessarily mean that you are not supposed to compose other elements within an Aggregate that don't need to be consistent after a transaction.
> After all, an Aggregate also models a conceptual whole. But you should be first and foremost concerned with transactional consistency. The outer boundary drawn around Aggregate Type 1 and Aggregate
> Type 2 represents a separate transaction that will be in control of atomically persisting each object cluster.
>
> Vernon, V. (2016) Domain-Driven Design Distilled, 1st ed. New York: Addison-Wesley Professional, p78.

> Aggregate is a pattern in Domain-Driven Design. A DDD aggregate is a cluster of domain objects that can be treated as a single unit. An example may be an order and its line-items, these will be
> separate objects, but it's useful to treat the order (together with its line items) as a single aggregate.
>
> [Fowler, Martin. "DDD_Aggregate", _martinfowler.com_, last edited on 08 Jun 2015](https://martinfowler.com/bliki/DomainDrivenDesign.html)

### Event Driven Architecture (EDA)

> Event-driven architecture (EDA) is a software architecture paradigm promoting the production, detection, consumption of, and reaction to events. An event can be defined as "a significant change in
> state".
>
> ["Event-driven architecture." _Wikipedia_, Wikimedia Foundation, last edited on 9 May 2021](https://en.wikipedia.org/wiki/Event-driven_architecture)

> Event-driven architecture refers to a system of loosely coupled microservices that exchange information between each other through the production and consumption of events. An event-driven system
> enables messages to be ingested into the event driven ecosystem and then broadcast out to whichever services are interested in receiving them.
>
> [Jansen, Grace & Saladas, Johanna. "Advantages of the event-driven architecture pattern." _developer.ibm.com_, IBM Developer, last edited on 12 May 2021](https://developer.ibm.com/articles/advantages-of-an-event-driven-architecture)

![](https://raw.githubusercontent.com/AntonioFalcaoJr/EventualShop/release/.assets/img/eda.png)  
[Fig. 15: Uit de Bos, Oskar. _A simple illustration of events using the publish/subscribe messagingmodel_](https://medium.com/swlh/the-engineers-guide-to-event-driven-architectures-benefits-and-challenges-3e96ded8568b)

### CQRS

> CQRS stands for Command and Query Responsibility Segregation, a pattern that separates read and update operations for a data store. Implementing CQRS in your application can maximize its
> performance, scalability, and security. The flexibility created by migrating to CQRS allows a system to better evolve over time and prevents update commands from causing merge conflicts at the
> domain level.
>
> Benefits of CQRS include:
>
> - **Independent scaling**. CQRS allows the read and write workloads to scale independently, and may result in fewer lock contentions.
> - **Optimized data schemas**. The read side can use a schema that is optimized for queries, while the write side uses a schema that is optimized for updates.
> - **Security**. It's easier to ensure that only the right domain entities are performing writes on the data.
> - **Separation of concerns**. Segregating the read and write sides can result in models that are more maintainable and flexible. Most of the complex business logic goes into the write model. The read model can be relatively simple.
> - **Simpler queries**. By storing a materialized view in the read database, the application can avoid complex joins when querying.

> ["What is the CQRS pattern?" _MSDN_, Microsoft Docs, last edited on 2 Nov 2020](https://docs.microsoft.com/en-us/azure/architecture/patterns/cqrs)

![](https://raw.githubusercontent.com/AntonioFalcaoJr/EventualShop/release/.assets/img/cqrs.png)   
[Fig. 21: Bürckel, Marco. _Some thoughts on using CQRS without Event Sourcing_](https://medium.com/@mbue/some-thoughts-on-using-cqrs-without-event-sourcing-938b878166a2)


![](https://raw.githubusercontent.com/AntonioFalcaoJr/EventualShop/release/.assets/img/cqrs.jpg) 
[Fig. 22: Go, Jayson. _From Monolith to Event-Driven: Finding Seams in Your Future Architecture_](https://www.eventstore.com/blog/what-is-event-sourcing)


### CQRS + Event Sourcing

> CQRS and Event Sourcing have a symbiotic relationship. CQRS allows Event Sourcing to be used as the data storage mechanism for the domain.
>
> Young Greg, 2012, _CQRS and Event Sourcing_, **CQRS Documents by Greg Young**, p50.
>
> The CQRS pattern is often used along with the Event Sourcing pattern. CQRS-based systems use separate read and write data models, each tailored to relevant tasks and often located in physically
> separate stores. When used with the Event Sourcing pattern, the store of events is the write model, and is the official source of information. The read model of a CQRS-based system provides
> materialized views of the data, typically as highly denormalized views. These views are tailored to the interfaces and display requirements of the application, which helps to maximize both display and query performance.
>
> ["Event Sourcing and CQRS pattern" _MSDN_, Microsoft Docs, last edited on 02 Nov 2020](https://docs.microsoft.com/en-us/azure/architecture/patterns/cqrs#event-sourcing-and-cqrs-pattern)

![](https://raw.githubusercontent.com/AntonioFalcaoJr/EventualShop/release/.assets/img/cqrs-eventsourcing-diagram.png)
[Fig. 24: Whittaker, Daniel. _CQRS + Event Sourcing – Step by Step_](https://danielwhittaker.me/2020/02/20/cqrs-step-step-guide-flow-typical-application)

![](https://raw.githubusercontent.com/AntonioFalcaoJr/EventualShop/release/.assets/img/cqrs-eventsourcing-flow.png)  
[Fig. 25: Katwatka, Piotr. _Event Sourcing with CQRS_](https://www.divante.com/blog/event-sourcing-open-loyalty-engineering)

### Clean Architecture

> Clean architecture is a software design philosophy that separates the elements of a design into ring levels. An important goal of clean architecture is to provide developers with a way to organize
> code in such a way that it encapsulates the business logic but keeps it separate from the delivery mechanism.
>
> The main rule of clean architecture is that code dependencies can only move from the outer levels inward. Code on the inner layers can have no knowledge of functions on the outer layers. The
> variables, functions and classes (any entities) that exist in the outer layers can not be mentioned in the more inward levels. It is recommended that data formats also stay separate between levels.
>
> ["Clean Architecture." _Whatis_, last edited on 10 Mar 2019](https://whatis.techtarget.com/definition/clean-architecture)

![](https://raw.githubusercontent.com/AntonioFalcaoJr/EventualShop/release/.assets/img/CleanArchitecture.jpg)  
[Fig. 28: C. Martin, Robert. _The Clean Architecture_](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)