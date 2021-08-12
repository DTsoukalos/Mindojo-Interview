# Mindojo-Interview
The interview that I had for the Mobile Game Developer position in Mindojo

It was a simple drawing game with specific rules like
A letter is consisting of 1 or more strokes, each consisting of 2 or more key points. For the purpose of this task, all strokes are in straight lines. For example, letter “A” could be defined as (0,0 is in the bottom-left):

- stroke 1: 
    - (0,0)
    - (3,9)
    - (6,0)
- stroke 2:
    - (1,3)
    - (5,3)
    - ![image](https://user-images.githubusercontent.com/51972234/129212602-3e38648c-7da3-4edf-a83f-dae60f8d7ef2.png)

User experience
- The user should see the letter on the screen as a thick dashed outline
- Every point should be a circle with a number inside (according to it’s global order number)
- The user must touch and draw th, each stroke with one touch (i.e. not removing the finger from the screen), and follow the outlined area.
- A valid stroke should start/end/pass through the relevant keypoints within X pixels.
- As user draws, the outlined area should be filled with that stroke
- If the user fails to draw a stroke, this stroke is reset and a message is shown “Please try again”. The failure should be detected immediately, not waiting for the user to release their finger.

The input of the code for each letter was a list of a Vector4 which the fist 2 number was the keypoint coordinates and the 4th number was about the number of the stroke that the key point belongs.
Example for letter A 

![image](https://user-images.githubusercontent.com/51972234/129213594-27ef92ba-5a30-4182-8df3-618d9dbf2374.png)

So my final results was like the picture below

![image](https://user-images.githubusercontent.com/51972234/129182211-11bf1587-962c-4718-9238-24555bb314ea.png)

In case of a curved letter I created a gravity keypoint (with is invisible). A gravity point makes a strait line between to points to a curved line. To define a key gravity point just you must put at the 3rd number -1.
For example, the S letter

![image](https://user-images.githubusercontent.com/51972234/129215707-c8fb4322-9fe7-43c9-b718-1d91b41e812a.png)
![image](https://user-images.githubusercontent.com/51972234/129215756-f76cc77a-76f1-4009-b1db-ff6d6ffa7d23.png)


