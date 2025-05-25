# How to Train and Evaluate a Regression Model

Regression models are a type of supervised machine learning models that can predict numeric values based on features and labels. In this post, I will explain the basic steps for training and evaluating a regression model, using a simple example of ice cream sales prediction.

## Training a Regression Model

The first step is to prepare some historic data that includes both features and labels. For our example, we will use the temperature (x) as the feature and the number of ice creams sold (y) as the label. Here is a sample of our data:

| Temperature (x) | Ice cream sales (y) |
| --------------- | ------------------- |
| 51              | 1                   |
| 52              | 0                   |
| 67              | 14                  |
| 65              | 14                  |
| 70              | 23                  |
| 69              | 20                  |
| 72              | 23                  |
| 75              | 26                  |
| 73              | 22                  |
| 81              | 30                  |
| 78              | 26                  |
| 83              | 36                  |

Next, we need to split the data into two subsets: one for training the model and one for validating the model. We will use 70% of the data for training and 30% for validation. Here is how our training data looks like:

| Temperature (x) | Ice cream sales (y) |
| --------------- | ------------------- |
| 51              | 1                   |
| 65              | 14                  |
| 69              | 20                  |
| 72              | 23                  |
| 75              | 26                  |
| 81              | 30                  |

Then, we need to choose an algorithm to fit the training data to a model. For our example, we will use linear regression, which works by deriving a function that produces a straight line through the intersections of the x and y values. The function can be expressed like this:

$$f(x) = x-50$$

We can use this function to predict the number of ice creams sold for any given temperature. For example, if the temperature is 77 degrees, we can calculate 77-50 and predict that we will sell 27 ice creams.

## Evaluating a Regression Model

To validate the model and evaluate how well it predicts, we need to use the data that we held back for validation. Here is how our validation data looks like:

| Temperature (x) | Ice cream sales (y) |
| --------------- | ------------------- |
| 52              | 0                   |
| 67              | 14                  |
| 70              | 23                  |
| 73              | 22                  |
| 78              | 26                  |
| 83              | 36                  |

We can use the model to predict the label for each of the observations in the validation data based on the feature value, and then compare the predicted label with the actual label. Here are the predictions and the errors:

| Temperature (x) | Actual sales (y) | Predicted sales (ŷ) | Error (y-ŷ) |
| --------------- | ---------------- | ------------------- | ----------- |
| 52              | 0                | 2                   | -2          |
| 67              | 14               | 17                  | -3          |
| 70              | 23               | 20                  | 3           |
| 73              | 22               | 23                  | -1          |
| 78              | 26               | 28                  | -2          |
| 83              | 36               | 33                  | 3           |

Based on the errors, we can calculate some common metrics that are used to evaluate a regression model, such as:

- Mean Absolute Error (MAE): The average of the absolute errors. In our example, the MAE is 2.33.
- Mean Squared Error (MSE): The average of the squared errors. In our example, the MSE is 6.
- Root Mean Squared Error (RMSE): The square root of the MSE. In our example, the RMSE is 2.45.
- Coefficient of Determination (R2): A value between 0 and 1 that measures the proportion of variance explained by the model. In our example, the R2 is 0.95.

These metrics can help us assess how well the model fits the validation data and how accurate the predictions are.

## Iterative Training

The metrics described above are commonly used to evaluate a regression model. In most real-world scenarios, a data scientist will use an iterative process to repeatedly train and evaluate a model, varying:

- Feature selection and preparation
- Algorithm selection
- Algorithm parameters

After multiple iterations, the model that results in the best evaluation metric that is acceptable for the specific scenario is selected.

I hope this post gave you a clear overview of how to train and evaluate a regression model. If you have any questions or feedback, please leave a comment below. Thank you for reading!
