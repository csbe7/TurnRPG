extends Button

class_name choice_button

var choice_index : int

signal Selected(index : int)

func _ready():
	connect("button_down", Select)

func Select():
	emit_signal("Selected", choice_index)
