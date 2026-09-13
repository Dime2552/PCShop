import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule, DatePipe, CurrencyPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { ButtonModule } from 'primeng/button';
import { OrderService } from '../../../core/services/order.service';
import { OrderDto } from '../../../core/models/order.model';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-my-orders',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    DatePipe,
    CurrencyPipe,
    TableModule,
    TagModule,
    ButtonModule
  ],
  templateUrl: './my-orders.html'
})
export class MyOrdersComponent implements OnInit {
  private orderService = inject(OrderService);
  private messageService = inject(MessageService);

  orders = signal<OrderDto[]>([]);
  isLoading = signal(true);
  expandedRows: { [s: string]: boolean } = {};

  toggleOrder(order: OrderDto) {
    if (!order?.orderId) return;
    this.expandedRows[order.orderId] = !this.expandedRows[order.orderId];
    this.expandedRows = { ...this.expandedRows };
  }

  isOrderExpanded(order: OrderDto): boolean {
    return !!(order?.orderId && this.expandedRows[order.orderId]);
  }

  ngOnInit() {
    this.loadOrders();
  }

  loadOrders() {
    this.isLoading.set(true);
    this.orderService.getMyOrders().subscribe({
      next: (data) => {
        this.orders.set(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.isLoading.set(false);
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: err.error?.detail || 'Failed to load order history'
        });
      }
    });
  }

  getStatusSeverity(status: string): 'warn' | 'success' | 'info' | 'danger' | 'secondary' {
    switch (status?.toLowerCase()) {
      case 'pending':
        return 'warn';
      case 'paid':
        return 'success';
      case 'shipped':
        return 'info';
      case 'cancelled':
        return 'danger';
      default:
        return 'secondary';
    }
  }
}
