export interface ProductAttributeItem {
  key: string;
  value: string;
}

export interface ProductDto {
  id: string;
  name: string;
  brand: string;
  price: number;
  discountPrice?: number;
  mainImageUrl: string;
  stockQuantity: number;
  description?: string;
  imageUrls?: string[];
  attributes?: ProductAttributeItem[] | Record<string, string>;
}