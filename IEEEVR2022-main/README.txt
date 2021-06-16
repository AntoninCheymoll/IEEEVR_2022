Trying the embodiements: 

	In order to try the different embodiements, open the "Hand examples" scene in the "Scenes" folder. 

	Here are the commands to use it: 

		Space: change to the next dimorphism mode
		M: Put the scene in full screen or leave full screen
		C: For the 6th finger embodiement, change the retargetting matching (there is two different retargetting which can be applied, which will be choose during the experiment corresponding to what the user will need to touch during the task.)

	Advice: 
	
		You can use the leap motion only to try it, but try to keep it around your face position (the postion it will be positionned to when we will use HMD), if not the tracking quality is lowered. 

About the experience: 
	
	-The whole experiment protocol is on the Dropbox page: https://paper.dropbox.com/doc/New-Project-Nami-Yuutaro-Antonin--BLAVL0K8572Ph20B~kDMwCJzAg-0LdvWHBUZjkthjjiemqJG
	
	- Commands:
		M: Put the scene in full screen or leave full screen
		W: Start the task phase
		Q: Validate an answer or notified that the participant is ready to start the questionnaire/ Proprioceptive test (Note: this cmd is used to replace the A button of the controller during the experiment, it can be use for debugging purpose, but is not supposed to be used during the experiment)
		Directional arrows: Select the questionnaire answer or move the Poprioceptive drift panel (Note: this cmd is used to replace the joystick button of the controller during the experiment, it can be use for debugging purpose, but is not supposed to be used during the experiment)
		P: Print the current csv file 	
	
	- File generation
		- The results file are generated automatically in the "experiment files" folder(IMPORTANT: If the experiment is runned for the same participant twice, the new file will overwrite the previous one)
		- The results files anem are written in this format: Result_"Participant number"_"Task type (Q = questionaire, PD = proprioceptive task)"
		- The condition orders is written in the "RandomizedSamples" file and were generated with the script "ExperimentFileGenerator", a new one is generated everytime the file is missing.

	- Main experiment parameters (X = parameters which should be changed (not only for debugging purpose))
		In Experiment Manager script (in ExperimentScriptsHolder)
			- Example : is equal to true when the experiment is in exemple phase (at the beggining of the experiment), set it to false when you start the experiment if you want to sart directly without the example session
			- Current state : State of one sample progression.
			- Current phase : Whether Offset (proprioceptive drift test) or noOffset (questionnaire), the experiment runs first all the no offset sample, then all of the no offset samples. 
			- Task duration : Duration of the visuomotor or self touch task, should be set to 90. 
			- Break duration : Duration of the break between all samples, should be set to 45.  
			X Participant number : number of the participant from 0 to 19
			- Current transform : Within one group of sample of the same conditions (visuomotor vs self touch and offset vs no offset) num of the transform (= num of the sample) from 0 to 3 (set to -1 at the experiment start)
			- Current block : correspond to the num of the block between visuomotor and self touch, within each  offset/noOffset part (within each two parts there is two blocks)

	- Hands game objects: 
		The hands game objects are present in the "Control" empty game object, they are splited into 3 cathegories: 
			-Display: the hand which is visible in the scene
			-Alternative: Only displayed for the 6 fingers condition to diplay one additional finger. 
			-Invisible: Not visible in the scene, is used as a reference for the retargetting and for the proprioceptive drift test. 
		
	- Remarks: 
		- the proprioceptive drift test cannot be computed if the left hand is not tracked (doesnt mean it is displayed). If the participant presses A but the experiment is not continuing, it maybe be because the left hand is not tracked (the leap motion is not in front of the hand)
		- Between each samples, its better if the participant remove the HMD and touch his own hands. To remove the embodiement effects of the previous sample in the next one.
		- During the self touch task, the participant has to follow a path displayed on his left hand with his right index. The right index has to be close enough to the most colorfull sphere, to make it disapear, and pass to the next sphere. When he touched all the sphere, he has to split his hand with enough distance, to make the next one appear.
		- If the path to follow by the participants on the hand (red dots) is bugging for any reason and cannot be completed. Ask the participant to split his hands, and press the "Reset Path" bolean on the Experiment Manager script (in ExperimentScriptsHolder).
		- Its better to remind the participant that for the path following task, the speed of the task completion is not recorded, and to incitate them to complete the path slowly, in order to not create tracking issue, and to enmpower the embodiement
		- Not to forget to remind the participant to not touch his thumb during the experiment. 
		- The tracking is better if the participant look down when he looks at his hands.
		- If the leap motions hand get too close of the occulus "guardian" border, it may move the camera, try to have a big enough guardian. 
 
	- 