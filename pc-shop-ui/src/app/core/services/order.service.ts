import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { OrderDto } from '../models/order.model';

export interface Address {
    country: string;
    city: string;
    street: string;
    zipCode: string;
}

export interface CreateOrderRequest {
    shippingAddress: Address;
    shippingMethod: string;
}

export interface CreateOrderResponse {
    orderId: string;
    checkoutUrl: string;
}

@Injectable({ providedIn: 'root' })
export class OrderService {
    private http = inject(HttpClient);
    private apiUrl = `${environment.apiUrl}/orders`;

    createOrder(data: CreateOrderRequest) {
        return this.http.post<CreateOrderResponse>(this.apiUrl, data);
    }

    getMyOrders(): Observable<OrderDto[]> {
        return this.http.get<any[]>(this.apiUrl).pipe(
            map(orders => orders.map(o => ({
                orderId: o.orderId ?? o.OrderId ?? o.id ?? o.Id ?? '',
                totalAmount: o.totalAmount ?? o.TotalAmount ?? 0,
                shippingCost: o.shippingCost ?? o.ShippingCost ?? 0,
                status: o.status ?? o.Status ?? 'Pending',
                createdAt: o.createdAt ?? o.CreatedAt ?? '',
                items: (o.items ?? o.Items ?? []).map((i: any) => ({
                    productId: i.productId ?? i.ProductId ?? '',
                    productName: i.productName ?? i.ProductName ?? 'Unknown Product',
                    quantity: i.quantity ?? i.Quantity ?? 1,
                    unitPrice: i.unitPrice ?? i.UnitPrice ?? 0
                }))
            })))
        );
    }
}