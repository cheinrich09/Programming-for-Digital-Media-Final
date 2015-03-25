2/21/13 - Final Project: 3d Sailboats


Advanced Behaviors:
1. Containment:
The first major behavior implemented in my project, and the one which I most wanted to get right, as containment is the behavior which gave me the most trouble when working on the 2d version of this project. And despite consistently trying to do it the hardest way possible, I actually got containment working fairly quickly. Each boat has an array of three rays, called feelers, which test to see if the boat is going to hit the terrain. Once the raycast returns true, meaning that the boat is going to collide with the terrain within the given distance, the behavior then takes the normal from the point where the raycast hit the terrain, as given by the RaycastHit object, and projects that on the perpindicular of the forward velocity, as given by transform.right;
2. Wander
Wander is carried over from my 2d project, and was fairly simple to implement, just bounding the offset to  a circle in front of the boat, and then adjusting this offset a little bit each time.
3. Flow Field
Flow Field was the newest steering behavior to be added, and was implemented by creating a series of empty game objects which acted as the points in the flowfield, and then rotating their transforms to get the direction, and adding all of these to an array in the manager class. Originally I intended this to be used to simulate wind, but I began to have issues with the end resuslts of the behaviors, as it seemed to be too similar to regular flocking with the boats just sticking to the edges of the lake, so I instead used the flowfield to push the boats more towards the center, which created a more interesting behavior overall. This is why the flow field methods aligns to the right of the transform instead of the forward. 

Other notes:
I had some interesting issues with models, first with finding a model which would actually cooperate with what I was inteading (a lot of the models I tried initially were too high poly and caused my computer to lag when they were implemented) and then with having the model actually do what I wanted it to do, as just using the models transform would cause it to move perpindicular to what I wanted, which was solved by putting it inside another object. also, I had some issues with setting up the avatar, so unfortuneatly its collisions with the terrain aren't actually working, as I couldn't use the normal move command, as it just sat there, instead I'm just manually moving its transform, which ignores collisions.


Credits
1. Model Source
skerta from 3dxtras.com: http://www.3dxtras.com/3dxtras-free-3d-models-details.asp?prodid=7900#