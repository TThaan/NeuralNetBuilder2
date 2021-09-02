When an array represents a matrix: 
- the length of its first dimension is m and its index is j
- the length of its second dimension is n and its index is k.

w^l_jk means:
w = (a single) weight (not a matrix)
l = layer that the weight is "going to" (l-1 = layer that the weight is "coming from")
j = index of the "receiving" neuron in layer l (out of m neurons there)
k = index of the "sending" neuron in layer l-1 (out of n neurons there)

Example:
layer l   => 3 neurons and 
layer l-1 => 2 neurons		->	w^l: m = 3 (rec), n = 2 (send)