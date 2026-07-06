import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { environment } from '../../../environments/environment';

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

@Injectable({ providedIn: 'root' })
export class OrderService {
    private http = inject(HttpClient);
    private apiUrl = `${environment.apiUrl}/orders`;

    createOrder(data: CreateOrderRequest) {
        return this.http.post<string>(this.apiUrl, data);
    }
}