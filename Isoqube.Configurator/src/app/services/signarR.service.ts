import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Observable, Subject } from 'rxjs';
import { TopicNotification } from '../models/http';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})

export class SignalRService {
  
  private hubConnection: signalR.HubConnection | undefined;
  private messageSubject = new Subject<TopicNotification>();

  constructor() {
    this.startConnection();
  }

  private startConnection() {
    
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl( `${environment.API_DEFAULT_INGESTION}/notifyclientshub`)
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('SignalR Connected'))
      .catch((err) => console.error('Connection Error:', err));

    this.hubConnection.on('listentonotifications', (message: TopicNotification) => {
      this.messageSubject.next(message);
    });
  }

  getMessages(): Observable<TopicNotification> {
    return this.messageSubject.asObservable();
  }
}
