---
layout: post
title: "Machine Learning Init. Linear Regression. Gradient Descent"
date: 2016-11-14

tags: machine learning
categories: learning
---
The information is from this [course](https://www.coursera.org/learn/machine-learning/).

[Machine learning](https://en.wikipedia.org/wiki/Machine_learning) is the subfield of computer science that "gives computers the ability to learn without being explicitly programmed" (Arthur Samuel, 1959).

In general, any machine learning problem can be assigned to one of two broad classifications:
* supervised learning ("regression" and "classification")
* unsupervised learning

In supervised learning, we are given a data set and already know what our correct output should look like, having the idea that there is a relationship between the input and the output.

Unsupervised learning, on the other hand, allows us to approach problems with little or no idea what our results should look like. We can derive structure from data where we don't necessarily know the effect of the variables.

### [Linear Regression](https://en.wikipedia.org/wiki/Linear_regression) with One Variable

In statistics, linear regression is an approach for modeling the relationship between a scalar dependent variable y and one or more explanatory variables (or independent variables) denoted X. The case of one explanatory variable is called simple linear regression. For more than one explanatory variable, the process is called multiple linear regression.

![Linear regression](https://upload.wikimedia.org/wikipedia/commons/thumb/3/3a/Linear_regression.svg/438px-Linear_regression.svg.png)

Hypothesis function has the general form:

$$\[\hat{y} = h_\theta(x) = \theta_0 + \theta_1 x\]$$

### Cost Function

We can measure the accuracy of our hypothesis function by using a cost function. This takes an average (actually a fancier version of an average) of all the results of the hypothesis with inputs from x's compared to the actual output y's.

$$\[J(\theta_0, \theta_1) = \dfrac {1}{2m} \displaystyle \sum _{i=1}^m \left ( \hat{y}_{i}- y_{i} \right)^2  = \dfrac {1}{2m} \displaystyle \sum _{i=1}^m \left (h_\theta (x_{i}) - y_{i} \right)^2\]$$

If we try to think of it in visual terms, our training data set is scattered on the x-y plane. We are trying to make straight line (defined by $h_\theta(x)$) which passes through this scattered set of data. Our objective is to get the best possible line. The best possible line will be such so that the average squared vertical distances of the scattered points from the line will be the least. In the best case, the line should pass through all the points of our training data set. In such a case the value of $J(\theta_0), (\theta_1 )$ will be 0.

### Gradient Descent

There is hypothesis function and there are a set of {x, y} values, so we need to find $\theta_0$ and $\theta_1$.

The gradient descent algorithm is:

$$\[\theta_j := \theta_j - \alpha \frac{\partial}{\partial \theta_j} J(\theta_0, \theta_1)\]$$

### Gradient Descent for Linear Regression

$$\[\begin{align*} \text{repeat until convergence: } \lbrace & \newline \theta_0 := & \theta_0 - \alpha \frac{1}{m} \sum\limits_{i=1}^{m}(h_\theta(x_{i}) - y_{i}) \newline \theta_1 := & \theta_1 - \alpha \frac{1}{m} \sum\limits_{i=1}^{m}\left((h_\theta(x_{i}) - y_{i}) x_{i}\right) \newline \rbrace& \end{align*}\]$$
