import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { AuditLog } from '../models/http';

@Injectable({
  providedIn: 'root'
})

export class DefaultService {
  constructor(private http: HttpClient) { }

  public auditLog(auditLog: AuditLog) {
    return this.http.post(`${environment.API_ENDPOINT}/api/default`, auditLog);
  }
}
