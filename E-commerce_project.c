#include <stdio.h>
#include <stdlib.h>
#include <string.h>

typedef struct Order {
    int orderId;
    char customerName[50];
    int priority; 
    struct Order* next;
} Order;

typedef struct QueueNode {
    Order* order;
    struct QueueNode* next;
} QueueNode;

QueueNode* front = NULL;
QueueNode* rear = NULL;


Order* completedOrders = NULL;

#define MAX_HEAP_SIZE 100
Order* orderHeap[MAX_HEAP_SIZE];
int heapSize = 0;

void remove_newline(char *str) {
    size_t len = strlen(str);
    if(len > 0 && str[len - 1] == '\n') {
        str[len - 1] = '\0';
    }
}

void enqueue(Order* newOrder) {
    QueueNode* temp = (QueueNode*)malloc(sizeof(QueueNode));
    temp->order = newOrder;
    temp->next = NULL;
    if (rear == NULL) {
        front = rear = temp;
    } else {
        rear->next = temp;
        rear = temp;
    }
    printf("Order %d added to queue.\n", newOrder->orderId);
}

Order* dequeue() {
    if (front == NULL) return NULL;
    QueueNode* temp = front;
    front = front->next;
    if (front == NULL) rear = NULL;
    Order* ord = temp->order;
    free(temp);
    return ord;
}

void swap(Order** a, Order** b) {
    Order* temp = *a;
    *a = *b;
    *b = temp;
}

void heapifyUp(int index) {
    while (index > 0 && orderHeap[(index - 1) / 2]->priority > orderHeap[index]->priority) {
        swap(&orderHeap[index], &orderHeap[(index - 1) / 2]);
        index = (index - 1) / 2;
    }
}

void insertHeap(Order* ord) {
    if (heapSize >= MAX_HEAP_SIZE) return;
    orderHeap[heapSize] = ord;
    heapifyUp(heapSize);
    heapSize++;
    printf("Priority order %d added with priority %d.\n", ord->orderId, ord->priority);
}

void heapifyDown(int index) {
    int smallest = index;
    int left = 2 * index + 1;
    int right = 2 * index + 2;

    if (left < heapSize && orderHeap[left]->priority < orderHeap[smallest]->priority)
        smallest = left;
    if (right < heapSize && orderHeap[right]->priority < orderHeap[smallest]->priority)
        smallest = right;

    if (smallest != index) {
        swap(&orderHeap[index], &orderHeap[smallest]);
        heapifyDown(smallest);
    }
}

Order* extractMinHeap() {
    if (heapSize <= 0) return NULL;
    Order* min = orderHeap[0];
    orderHeap[0] = orderHeap[--heapSize];
    heapifyDown(0);
    return min;
}

void addCompletedOrder(Order* ord) {
    ord->next = completedOrders;
    completedOrders = ord;
    printf("Order %d completed and stored.\n", ord->orderId);
}

void displayCompletedOrders() {
    printf("\nCompleted Orders:\n");
    if (completedOrders == NULL) {
        printf("No completed orders yet.\n");
        return;
    }
    Order* temp = completedOrders;
    while (temp != NULL) {
        printf("OrderID: %d, Customer: %s\n", temp->orderId, temp->customerName);
        temp = temp->next;
    }
}

void menu() {
    int choice;
    int orderId = 1;
    int running = 1;
    char input[100];

    while (running) {
        printf("\n===== E-Commerce Order System =====\n");
        printf("1. Add Order\n");
        printf("2. Process Next Order (Queue)\n");
        printf("3. Add Priority Order\n");
        printf("4. Process Priority Order (Heap)\n");
        printf("5. Show Completed Orders\n");
        printf("6. Exit\n");
        printf("Enter your choice: ");

        if (fgets(input, sizeof(input), stdin) == NULL) {
            printf("Error reading input. Exiting.\n");
            break;
        }
        remove_newline(input);
        if (sscanf(input, "%d", &choice) != 1) {
            printf("Invalid input. Try again.\n");
            continue;
        }

        switch (choice) {
            case 1: {
                Order* newOrder = (Order*)malloc(sizeof(Order));
                newOrder->orderId = orderId++;
                printf("Enter customer name: ");
                if (fgets(newOrder->customerName, sizeof(newOrder->customerName), stdin) != NULL) {
                    remove_newline(newOrder->customerName);
                }
                newOrder->priority = 100; // default low priority
                newOrder->next = NULL;
                enqueue(newOrder);
                break;
            }
            case 2: {
                Order* ord = dequeue();
                if (ord) addCompletedOrder(ord);
                else printf("No orders in queue.\n");
                break;
            }
            case 3: {
                Order* priorityOrder = (Order*)malloc(sizeof(Order));
                priorityOrder->orderId = orderId++;
                printf("Enter customer name: ");
                if (fgets(priorityOrder->customerName, sizeof(priorityOrder->customerName), stdin) != NULL) {
                    remove_newline(priorityOrder->customerName);
                }
                printf("Enter priority (1-highest, 100-lowest): ");
                if (fgets(input, sizeof(input), stdin) != NULL) {
                    if (sscanf(input, "%d", &priorityOrder->priority) != 1) {
                        printf("Invalid priority. Setting default priority 100.\n");
                        priorityOrder->priority = 100;
                    }
                } else {
                    priorityOrder->priority = 100;
                }
                priorityOrder->next = NULL;
                insertHeap(priorityOrder);
                break;
            }
            case 4: {
                Order* ord = extractMinHeap();
                if (ord) addCompletedOrder(ord);
                else printf("No priority orders to process.\n");
                break;
            }
            case 5:
                displayCompletedOrders();
                break;
            case 6:
                printf("Exiting system...\n");
                running = 0;
                break;
            default:
                printf("Invalid choice. Try again.\n");
        }
    }
}

int main() {
    menu();
    return 0;
}
