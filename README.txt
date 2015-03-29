Since the document only implies that the customer events in the 
input file will be in temporal order, I chose a load-it-all 
approach, so I can sort the events by time in case you throw a 
file at it with out-of-order events.
If I could depend on events being in temporal order, I could use 
a streaming approach instead, which would allow the app to process 
input files of unlimited size, without getting an OutOfMemoryException.

DI / IoC using MEF:
I used MEF to reflect the idea of laying new foundations with good 
modularity and separation of concerns approaches, even though it is
complete overkill for something of this scope.

There are any number of ways to write this solution.  Since this isn't
a class library, the models are not designed to protect the integrity of 
their state or perform any significant actions themselves. They are pretty 
much dumb property bags that are manipulated by the simulator service.  
The simulator service contains most of the logic involved.

I put in a single unit test to prove that I can use them.
If extensive tests were expected, let me know.
