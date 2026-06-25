import { HttpInterceptorFn } from '@angular/common/http';

export const sessionInterceptor: HttpInterceptorFn = (req, next) => {
  let sessionId = localStorage.getItem('session_id');
  
  // Generate session ID for guest cart
  if (!sessionId) {
    sessionId = crypto.randomUUID();
    localStorage.setItem('session_id', sessionId);
  }

  if (req.url.includes('localhost:7120')) {
    let headers = req.headers.set('x-session-id', sessionId);

    // Add JWT Token if user is logged in
    const token = localStorage.getItem('token');
    if (token) {
      headers = headers.set('Authorization', `Bearer ${token}`);
    }

    const modifiedReq = req.clone({ headers });
    return next(modifiedReq);
  }

  return next(req);
};