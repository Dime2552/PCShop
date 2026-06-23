import { HttpInterceptorFn } from '@angular/common/http';

export const sessionInterceptor: HttpInterceptorFn = (req, next) => {
  let sessionId = localStorage.getItem('session_id');
  
  if (!sessionId) {
    sessionId = crypto.randomUUID();
    localStorage.setItem('session_id', sessionId);
  }

  if (req.url.includes('localhost:7120')) {
    const modifiedReq = req.clone({
      headers: req.headers.set('x-session-id', sessionId)
    });
    return next(modifiedReq);
  }

  return next(req);
};