import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule, DatePipe, CurrencyPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { MessageService } from 'primeng/api';
import { OrderService } from '../../../core/services/order.service';
import { AdminOrderDto } from '../../../core/models/order.model';

@Component({
  selector: 'app-admin-order-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    DatePipe,
    CurrencyPipe,
    TableModule,
    TagModule,
    ButtonModule,
    DialogModule,
    InputTextModule
  ],
  templateUrl: './admin-order-list.html'
})
export class AdminOrderListComponent implements OnInit {
  private orderService = inject(OrderService);
  private messageService = inject(MessageService);

  orders = signal<AdminOrderDto[]>([]);
  isLoading = signal<boolean>(true);
  isSubmitting = signal<boolean>(false);

  // Dialog State
  isShipDialogOpen = signal<boolean>(false);
  selectedOrder = signal<AdminOrderDto | null>(null);
  trackingNumber = signal<string>('');
  trackingError = signal<string>('');

  ngOnInit() {
    this.loadOrders();
  }

  loadOrders() {
    this.isLoading.set(true);
    this.orderService.getAllOrders().subscribe({
      next: (data) => {
        this.orders.set(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.isLoading.set(false);
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: err.error?.detail || 'Failed to load orders'
        });
      }
    });
  }

  openShipDialog(order: AdminOrderDto) {
    this.selectedOrder.set(order);
    this.trackingNumber.set('');
    this.trackingError.set('');
    this.isShipDialogOpen.set(true);
  }

  closeShipDialog() {
    if (this.isSubmitting()) return;
    this.isShipDialogOpen.set(false);
    this.selectedOrder.set(null);
    this.trackingNumber.set('');
    this.trackingError.set('');
  }

  onVisibleChange(visible: boolean) {
    if (!visible) {
      this.closeShipDialog();
    }
  }

  onTrackingNumberChange(value: string) {
    this.trackingNumber.set(value);
    if (value.trim()) {
      this.trackingError.set('');
    }
  }

  submitShipOrder() {
    const order = this.selectedOrder();
    const tracking = this.trackingNumber().trim();

    if (!tracking) {
      this.trackingError.set('Tracking number is required');
      return;
    }

    if (!order) return;

    this.isSubmitting.set(true);
    this.orderService.updateOrderTracking(order.orderId, tracking).subscribe({
      next: () => {
        this.messageService.add({
          severity: 'success',
          summary: 'Success',
          detail: 'Order shipped and tracking number updated'
        });
        this.isSubmitting.set(false);
        this.closeShipDialog();
        this.loadOrders();
      },
      error: (err) => {
        this.isSubmitting.set(false);
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: err.error?.detail || 'Failed to update order tracking'
        });
      }
    });
  }

  isPaid(status: string): boolean {
    return status?.toLowerCase() === 'paid';
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
