---
layout: post
title: "Neural Networks Learning"
date: 2016-12-11

tags: machine learning
categories: learning
---
### Cost Function

a) L= total number of layers in the network

b) $$\(s_l\)$$ = number of units (not counting bias unit) in layer l

c) K= number of output units/classes

$$\[\begin{gather*}\large J(\Theta) = - \frac{1}{m} \sum_{i=1}^m \sum_{k=1}^K \left[y^{(i)}_k \log ((h_\Theta (x^{(i)}))_k) + (1 - y^{(i)}_k)\log (1 - (h_\Theta(x^{(i)}))_k)\right] + \frac{\lambda}{2m}\sum_{l=1}^{L-1} \sum_{i=1}^{s_l} \sum_{j=1}^{s_{l+1}} ( \Theta_{j,i}^{(l)})^2\end{gather*}\]$$

### Backpropagation Algorithm

"Backpropagation" is neural-network terminology for minimizing our cost function, just like what we were doing with gradient descent in logistic and linear regression.

In back propagation we're going to compute for every node:

$$\(\delta_j^{(l)}\)$$ - = "error" of node j in layer l

$$\(a_j^{(l)}\)$$ is activation node j in layer l.

For the last layer, we can compute the vector of delta values with:

$$\[\delta^{(L)} = a^{(L)} - y\]$$

To get the delta values of the layers before the last layer, we can use an equation that steps us back from right to left:

$$\[\delta^{(l)} = ((\Theta^{(l)})^T \delta^{(l+1)})\ .*\ g'(z^{(l)})\]$$

$$\[g'(z^{(l)}) = a^{(l)}\ .*\ (1 - a^{(l)})\]$$

### Backpropagation algorithm

Given training set $$\(\lbrace (x^{(1)}, y^{(1)}) \cdots (x^{(m)}, y^{(m)})\rbrace\)$$;

* Set $$\(\Delta^{(l)}_{i,j}\)$$= 0 for all (l,i,j)

For training example t =1 to m:

* Set $$\(a^{(1)} := x^{(t)}\)$$
* Perform forward propagation to compute $$\(a^{(l)}\)$$ for l=2,3,…,L
* Using $$\(y^{(t)}\)$$, compute $$\(\delta^{(L)} = a^{(L)} - y^{(t)}\)$$
* Compute $$\(\delta^{(L-1)}, \delta^{(L-2)},\dots,\delta^{(2)}\)$$ using $$\(\delta^{(l)} = ((\Theta^{(l)})^T \delta^{(l+1)})\ .*\ a^{(l)}\ .*\ (1 - a^{(l)})\)$$
* $$\(\Delta^{(l)}_{i,j} := \Delta^{(l)}_{i,j} + a_j^{(l)} \delta_i^{(l+1)}\)$$ or with vectorization, $$\(\Delta^{(l)} := \Delta^{(l)} + \delta^{(l+1)}(a^{(l)})^T\)$$
* $$\(D^{(l)}_{i,j} := \dfrac{1}{m}\left(\Delta^{(l)}_{i,j} + \lambda\Theta^{(l)}_{i,j}\right)\)$$ If j≠0 NOTE: Typo in lecture slide omits outside parentheses. This version * is correct.
* $$\(D^{(l)}_{i,j} := \dfrac{1}{m}\Delta^{(l)}_{i,j}\)$$ If j=0

### Gradient Checking

Gradient checking will assure that our backpropagation works as intended.

$$\[\dfrac{\partial}{\partial\Theta}J(\Theta) \approx \dfrac{J(\Theta + \epsilon) - J(\Theta - \epsilon)}{2\epsilon}\]$$

Once you've verified once that your backpropagation algorithm is correct, then you don't need to compute gradApprox again. The code to compute gradApprox is very slow.

### Random Initialization

Initializing all theta weights to zero does not work with neural networks. When we backpropagate, all nodes will update to the same value repeatedly.

### Summary

First, pick a network architecture; choose the layout of your neural network, including how many hidden units in each layer and how many layers total.

* Number of input units = dimension of features $$\(x^{(i)}\)$$
* Number of output units = number of classes
* Number of hidden units per layer = usually more the better (must balance with cost of computation as it increases with more hidden units)
* Defaults: 1 hidden layer. If more than 1 hidden layer, then the same number of units in every hidden * layer.

**Training a Neural Network**

1. Randomly initialize the weights
2. Implement forward propagation to get $$\(h_\theta(x^{(i)})\)$$
3. Implement the cost function
4. Implement backpropagation to compute partial derivatives
5. Use gradient checking to confirm that your backpropagation works. Then disable gradient checking.
6. Use gradient descent or a built-in optimization function to minimize the cost function with the weights in theta.

When we perform forward and back propagation, we loop on every training example:

```none
for i = 1:m,
   Perform forward propagation and backpropagation using example (x(i),y(i))
   (Get activations a(l) and delta terms d(l) for l = 2,...,L
```

More info:
[https://www.coursera.org/learn/machine-learning](https://www.coursera.org/learn/machine-learning)