---
layout: post
title: "Clustering"
date: 2016-09-27

tags: machine learning
categories: learning
---
Unsupervised learning is contrasted from supervised learning because it uses an unlabeled training set rather than a labeled one.

### K-Means Algorithm

1. Randomly initialize two points in the dataset called the cluster centroids.
2. Cluster assignment: assign all examples into one of two groups based on which cluster centroid the example is closest to.
3. Move centroid: compute the averages for all the points inside each of the two cluster centroid groups, then move the cluster centroid points to those averages.
4. Re-run (2) and (3) until we have found our clusters.

```none
Randomly initialize K cluster centroids mu(1), mu(2), ..., mu(K)
Repeat:
   for i = 1 to m:
      c(i):= index (from 1 to K) of cluster centroid closest to x(i)
   for k = 1 to K:
      mu(k):= average (mean) of points assigned to cluster k
```

The first for-loop is the 'Cluster Assignment' step. We make a vector c where c(i) represents the centroid assigned to example x(i).

$$\[c^{(i)} = argmin_k\ ||x^{(i)} - \mu_k||^2\]$$

$$\[||x^{(i)} - \mu_k|| = ||\quad\sqrt{(x_1^i - \mu_{1(k)})^2 + (x_2^i - \mu_{2(k)})^2 + (x_3^i - \mu_{3(k)})^2 + ...}\quad||\]$$

The second for-loop is the 'Move Centroid' step where we move each centroid to the average of its group.

$$\[\mu_k = \dfrac{1}{n}[x^{(k_1)} + x^{(k_2)} + \dots + x^{(k_n)}] \in \mathbb{R}^n\]$$

After a number of iterations the algorithm will converge, where new iterations do not affect the clusters.

### Optimization Objective

Using these variables we can define our cost function:

$$\[J(c^{(i)},\dots,c^{(m)},\mu_1,\dots,\mu_K) = \dfrac{1}{m}\sum_{i=1}^m ||x^{(i)} - \mu_{c^{(i)}}||^2\]$$

With k-means, it is not possible for the cost function to sometimes increase. It should always descend.

### Random Initialization

1. Have K<m. That is, make sure the number of your clusters is less than the number of your training examples.
2. Randomly pick K training examples. (Not mentioned in the lecture, but also be sure the selected examples are unique).
3. Set μ1,…,μK equal to these K examples.

### Choosing the Number of Clusters

Choosing K can be quite arbitrary and ambiguous.

A way to choose K is to observe how well k-means performs on a downstream purpose. In other words, you choose K that proves to be most useful for some goal you're trying to achieve from using these clusters.

### Dimensionality Reduction

Motivation I: Data Compression

1. We may want to reduce the dimension of our features if we have a lot of redundant data.
2. To do this, we find two highly correlated features, plot them, and make a new line that seems to describe both features accurately. We place all the new features on this single line.

Doing dimensionality reduction will reduce the total data we have to store in computer memory and will speed up our learning algorithm.

Motivation II: Visualization

It is not easy to visualize data that is more than three dimensions. We can reduce the dimensions of our data to 3 or less in order to plot it.

### Principal Component Analysis Problem Formulation

The most popular dimensionality reduction algorithm is Principal Component Analysis (PCA)

Problem formulation

Given two features, x1 and x2, we want to find a single line that effectively describes both features at once. We then map our old features onto this new line to get a new single feature.

The goal of PCA is to reduce the average of all the distances of every feature to the projection line. This is the projection error.

**PCA is not linear regression**

* In linear regression, we are minimizing the squared error from every point to our predictor line. These are vertical distances.
* In PCA, we are minimizing the shortest distance, or shortest orthogonal distances, to our data points.

### Principal Component Analysis Algorithm

* Given training set: x(1),x(2),…,x(m)
* Preprocess (feature scaling/mean normalization):

$$\[\mu_j = \dfrac{1}{m}\sum^m_{i=1}x_j^{(i)}\]$$

* Replace each $x_j^{(i)}$ with $x_j^{(i)} - \mu_j$
* If different features on different scales (e.g., x1 = size of house, x2 = number of bedrooms), scale features to have comparable range of values.

1. Compute "covariance matrix"

$$\[Sigma = \dfrac{1}{m}\sum^m_{i=1}(x^{(i)})(x^{(i)})^T\]$$

2. Compute "eigenvectors" of covariance matrix Σ

```none
[U,S,V] = svd(Sigma);
```

3. Take the first k columns of the U matrix and compute z

Summarize:

```none
Sigma = (1/m) * X' * X; % compute the covariance matrix
[U,S,V] = svd(Sigma);   % compute our projected directions
Ureduce = U(:,1:k);     % take the first k directions
Z = X * Ureduce;        % compute the projected data points
```

### Reconstruction from Compressed Representation

$$\[x_{approx}^{(1)} = U_{reduce} \cdot z^{(1)}\]$$

### Choosing the Number of Principal Components

Algorithm for choosing k

1. Try PCA with k=1,2,…
2. Compute $U_{reduce}, z, x$
3. Check the formula given above that 99% of the variance is retained. If not, go to step one and increase k.

### Advice for Applying PCA

* Compressions
* Reduce space of data
* Speed up algorithm
* Visualization of data

**Bad use of PCA**: trying to prevent overfitting.

Don't assume you need to do PCA. **Try your full machine learning algorithm without PCA first**. Then use PCA if you find that you need it.

More info:
[https://www.coursera.org/learn/machine-learning](https://www.coursera.org/learn/machine-learning)
