# Load necessary libraries
library(ggplot2)
library(reshape2)

# -----------------------------
# 1. Create the Dataset
# -----------------------------
students <- data.frame(
  Student_Name = c("Alice", "Bob", "Charlie", "Daisy", "Ethan"),
  Maths = c(88, 76, 95, 67, 90),
  Science = c(82, 89, 91, 73, 85),
  English = c(78, 72, 88, 84, 80),
  History = c(90, 85, 87, 70, 75)
)

print("Student Dataset:")
print(students)

# -----------------------------
# 2. Check for missing values
# -----------------------------
print("Check for missing values:")
sum(is.na(students))  # Should be 0 if no missing data

# -----------------------------
# 3. Summary Statistics
# -----------------------------
summary_stats <- data.frame(
  Subject = names(students)[2:5],
  Mean = sapply(students[2:5], mean),
  Median = sapply(students[2:5], median),
  SD = sapply(students[2:5], sd)
)

print("Summary Statistics:")
print(summary_stats)

# -----------------------------
# 4. Data Reshaping for ggplot2
# -----------------------------
students_long <- melt(students, id.vars = "Student_Name",
                      variable.name = "Subject", value.name = "Score")

# -----------------------------
# 5. Bar Plot of Scores
# -----------------------------
ggplot(students_long, aes(x = Student_Name, y = Score, fill = Subject)) +
  geom_bar(stat = "identity", position = "dodge") +
  ggtitle("Bar Plot: Student Scores by Subject") +
  theme_minimal()

# -----------------------------
# 6. Box Plot
# -----------------------------
ggplot(students_long, aes(x = Subject, y = Score, fill = Subject)) +
  geom_boxplot() +
  ggtitle("Box Plot: Distribution of Scores by Subject") +
  theme_minimal()

# -----------------------------
# 7. Histogram of Science Scores
# -----------------------------
hist(students$Science,
     main = "Histogram: Distribution of Science Scores",
     xlab = "Science Scores",
     col = "lightblue",
     border = "black")

# -----------------------------
# 8. Line Plot: Performance Trend
# -----------------------------
ggplot(students_long, aes(x = Subject, y = Score, group = Student_Name, color = Student_Name)) +
  geom_line(size = 1.2) +
  geom_point(size = 3) +
  ggtitle("Performance Trend Across Subjects") +
  theme_minimal()

# -----------------------------
# 9. Identify Top and Bottom Scorers
# -----------------------------
top_scorers <- sapply(students[2:5], function(x) students$Student_Name[which.max(x)])
bottom_scorers <- sapply(students[2:5], function(x) students$Student_Name[which.min(x)])

print("Top Scorers:")
print(top_scorers)

print("Bottom Scorers:")
print(bottom_scorers)
