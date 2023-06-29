---
layout: post
title: Linear Regression with Multiple Variables
date: 2016-11-24

tags: machine learning
categories: learning
---
Hypothesis function looks like:

$$\[h_\theta (x) = \theta_0 + \theta_1 x_1 + \theta_2 x_2 + \theta_3 x_3 + \cdots + \theta_n x_n\]$$

Vectorized version:

$$\[h_\theta(X) = X \theta\]$$

Vectorized version of cost function:

$$\[J(\theta) = \dfrac {1}{2m} (X\theta - \vec{y})^{T} (X\theta - \vec{y})\]$$

Gradient descent:

$$\[\begin{align*}& \text{repeat until convergence:} \; \lbrace \newline \; & \theta_j := \theta_j - \alpha \frac{1}{m} \sum\limits_{i=1}^{m} (h_\theta(x^{(i)}) - y^{(i)}) \cdot x_j^{(i)} \;  & \text{for j := 0..n}\newline \rbrace\end{align*}\]$$

Vectorized version:

$$\[theta := \theta - \frac{\alpha}{m} X^{T} (X\theta - \vec{y})\]$$

### Feature Normalization

We can speed up gradient descent by having each of our input values in roughly the same range. This is because θ will descend quickly on small ranges and slowly on large ranges, and so will oscillate inefficiently down to the optimum when the variables are very uneven.

Two techniques to help with this are feature scaling and mean normalization. Feature scaling involves dividing the input values by the range (i.e. the maximum value minus the minimum value) of the input variable, resulting in a new range of just 1. Mean normalization involves subtracting the average value for an input variable from the values for that input variable, resulting in a new average value for the input variable of just zero. To implement both of these techniques, adjust your input values as shown in this formula:

$$\[x_i := \dfrac{x_i - \mu_i}{s_i}\]$$

Where $μ_i$ is the average of all the values for feature (i) and $s_i$ is the range of values (max - min), or $s_i$ is the standard deviation.

### Normal Equation

The "Normal Equation" is a method of finding the optimum theta without iteration.

$$\[\theta = (X^T X)^{-1}X^T y\]$$

Basically it means that we are solving the equation to find minimum. Additional info is [here](http://eli.thegreenplace.net/2014/derivation-of-the-normal-equation-for-linear-regression).

More info:
[https://www.coursera.org/learn/machine-learning](https://www.coursera.org/learn/machine-learning)
