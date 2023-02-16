import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { bufferTime, concatWith, filter, retry, Subject, switchMap } from 'rxjs';
import { ConfigService } from './config.service';

@Injectable({
	providedIn: 'root',
})
export class LoggingService {
	private readonly logs$ = new Subject<LogMessage>();

	constructor(readonly http: HttpClient, readonly config: ConfigService) {
		this.logs$
			.pipe(
				filter(msg => msg.level >= config.current$.value.logLevelForBackend),
				bufferTime(2000)
			)
			.subscribe(logs => {
				if (logs.length > 0) {
					http
						.post('/api/logs', logs)
						.pipe(retry(3))
						.subscribe({
							error: err => console.error('Failed to send logs to server', err),
						});
				}
			});

		this.logs$
			.pipe(filter(msg => msg.level >= config.current$.value.logLevelForConsole))
			.subscribe(({ level, message, items }) => {
				switch (level) {
					case LogLevel.debug:
						console.debug(message, items);
						break;
					case LogLevel.error:
						console.error(message, items);
						break;
					case LogLevel.warn:
						console.warn(message, items);
						break;
					default:
					case LogLevel.info:
						console.log(message, items);
						break;
				}
			});
	}

	log(level: LogLevel, message: string, ...items: any[]) {
		this.logs$.next({ level, message, items });
	}

	debug(message: string, ...items: any[]) {
		this.logs$.next({ level: LogLevel.debug, message, items });
	}

	info(message: string, ...items: any[]) {
		this.logs$.next({ level: LogLevel.info, message, items });
	}

	warn(message: string, ...items: any[]) {
		this.logs$.next({ level: LogLevel.warn, message, items });
	}

	error(message: string, ...items: any[]) {
		this.logs$.next({ level: LogLevel.error, message, items });
	}
}

export interface LogMessage {
	level: LogLevel;
	message: string;
	items?: any[];
}

export enum LogLevel {
	none = 0,
	debug = 10,
	info = 20,
	warn = 30,
	error = 40,
}
