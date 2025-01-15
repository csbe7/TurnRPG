extends Control

class_name dialogue_box

@export var ez_dialogue : EzDialogue

@export var label : RichTextLabel
@export var choice_container : VBoxContainer
@export var choice_button_scene : PackedScene



@export var dialogue : JSON
@onready var state : Dictionary

signal EndOfDialogueReached


func _ready():
	TranslationServer.set_locale("eng")
	#LoadDialogue(dialogue, state)

func LoadDialogue(dialogue_json : JSON, dialogue_state : Dictionary):
	state = dialogue_state
	ez_dialogue.start_dialogue(dialogue_json, dialogue_state)
	#for key in dialogue_state.keys():
	#	print(key)
	pass

func SetText(text : String):
	label.text = ParseTranslation(text);

func AddChoice(index : int, choice_text : String):
	var cb = choice_button_scene.instantiate()
	cb.choice_index = index
	cb.text = ParseTranslation(tr(choice_text))
	cb.connect("Selected", OnChoiceSelected)
	choice_container.add_child(cb)

func ClearDialogue():
	label.text = ""
	for child in choice_container.get_children():
		if (child is choice_button): child.queue_free()
	pass


func OnChoiceSelected(index : int):
	ez_dialogue.next(index)

func on_custom_signal_received(_value):
	pass # Replace with function body.


func on_dialogue_generated(response : DialogueResponse):
	ClearDialogue()
	SetText(tr(response.text))
	var i : int = 0 
	for choice in response.choices:
		AddChoice(i, choice)
		i = i + 1
	if (response.eod_reached):
		emit_signal("EndOfDialogueReached")

func ParseTranslation(toParse : String):
	var found : int = 0
	while(found != -1 && found+1 < toParse.length()):
		found = toParse.find("%", found+1)
		if (found == -1): continue
		var c : String = "a"
		var toReplace : String
		var offset : int = 1
		
		while (found + offset < toParse.length() && c != "" && c!= "%"):
			c = toParse[found + offset]
			if (c == "" || c == "%"): continue
			toReplace = toReplace + c
			offset = offset + 1
		
		if (state.has(toReplace)):
			if (state[toReplace] is String):
				toParse = toParse.replace("%" + toReplace + "%", state[toReplace])
			else: if (state[toReplace] is int):
				toParse = toParse.replace("%" + toReplace, state[toReplace].to_string())
	return toParse
