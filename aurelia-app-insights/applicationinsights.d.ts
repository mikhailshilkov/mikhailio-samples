declare module M.Microsoft.ApplicationInsights {
    enum LoggingSeverity {
        CRITICAL = 0,
        WARNING = 1,
    }
    enum _InternalMessageId {
        NONUSRACT_BrowserDoesNotSupportLocalStorage = 0,
        NONUSRACT_BrowserCannotReadLocalStorage = 1,
        NONUSRACT_BrowserCannotReadSessionStorage = 2,
        NONUSRACT_BrowserCannotWriteLocalStorage = 3,
        NONUSRACT_BrowserCannotWriteSessionStorage = 4,
        NONUSRACT_BrowserFailedRemovalFromLocalStorage = 5,
        NONUSRACT_BrowserFailedRemovalFromSessionStorage = 6,
        NONUSRACT_CannotSendEmptyTelemetry = 7,
        NONUSRACT_ClientPerformanceMathError = 8,
        NONUSRACT_ErrorParsingAISessionCookie = 9,
        NONUSRACT_ErrorPVCalc = 10,
        NONUSRACT_ExceptionWhileLoggingError = 11,
        NONUSRACT_FailedAddingTelemetryToBuffer = 12,
        NONUSRACT_FailedMonitorAjaxAbort = 13,
        NONUSRACT_FailedMonitorAjaxDur = 14,
        NONUSRACT_FailedMonitorAjaxOpen = 15,
        NONUSRACT_FailedMonitorAjaxRSC = 16,
        NONUSRACT_FailedMonitorAjaxSend = 17,
        NONUSRACT_FailedToAddHandlerForOnBeforeUnload = 18,
        NONUSRACT_FailedToSendQueuedTelemetry = 19,
        NONUSRACT_FailedToReportDataLoss = 20,
        NONUSRACT_FlushFailed = 21,
        NONUSRACT_MessageLimitPerPVExceeded = 22,
        NONUSRACT_MissingRequiredFieldSpecification = 23,
        NONUSRACT_NavigationTimingNotSupported = 24,
        NONUSRACT_OnError = 25,
        NONUSRACT_SessionRenewalDateIsZero = 26,
        NONUSRACT_SenderNotInitialized = 27,
        NONUSRACT_StartTrackEventFailed = 28,
        NONUSRACT_StopTrackEventFailed = 29,
        NONUSRACT_StartTrackFailed = 30,
        NONUSRACT_StopTrackFailed = 31,
        NONUSRACT_TelemetrySampledAndNotSent = 32,
        NONUSRACT_TrackEventFailed = 33,
        NONUSRACT_TrackExceptionFailed = 34,
        NONUSRACT_TrackMetricFailed = 35,
        NONUSRACT_TrackPVFailed = 36,
        NONUSRACT_TrackPVFailedCalc = 37,
        NONUSRACT_TrackTraceFailed = 38,
        NONUSRACT_TransmissionFailed = 39,
        NONUSRACT_FailedToSetStorageBuffer = 40,
        NONUSRACT_FailedToRestoreStorageBuffer = 41,
        NONUSRACT_InvalidBackendResponse = 42,
        USRACT_CannotSerializeObject = 43,
        USRACT_CannotSerializeObjectNonSerializable = 44,
        USRACT_CircularReferenceDetected = 45,
        USRACT_ClearAuthContextFailed = 46,
        USRACT_ExceptionTruncated = 47,
        USRACT_IllegalCharsInName = 48,
        USRACT_ItemNotInArray = 49,
        USRACT_MaxAjaxPerPVExceeded = 50,
        USRACT_MessageTruncated = 51,
        USRACT_NameTooLong = 52,
        USRACT_SampleRateOutOfRange = 53,
        USRACT_SetAuthContextFailed = 54,
        USRACT_SetAuthContextFailedAccountName = 55,
        USRACT_StringValueTooLong = 56,
        USRACT_StartCalledMoreThanOnce = 57,
        USRACT_StopCalledWithoutStart = 58,
        USRACT_TelemetryInitializerFailed = 59,
        USRACT_TrackArgumentsNotSpecified = 60,
        USRACT_UrlTooLong = 61,
        USRACT_SessionStorageBufferFull = 62,
        USRACT_CannotAccessCookie = 63,
    }
    class _InternalLogMessage {
        message: string;
        messageId: _InternalMessageId;
        constructor(msgId: _InternalMessageId, msg: string, properties?: Object);
        private static sanitizeDiagnosticText(text);
    }
    class _InternalLogging {
        private static AiUserActionablePrefix;
        private static AIInternalMessagePrefix;
        private static AiNonUserActionablePrefix;
        static enableDebugExceptions: () => boolean;
        static verboseLogging: () => boolean;
        static queue: any[];
        private static MAX_INTERNAL_MESSAGE_LIMIT;
        private static _messageCount;
        private static _messageLogged;
        static throwInternalNonUserActionable(severity: LoggingSeverity, message: _InternalLogMessage): void;
        static throwInternalUserActionable(severity: LoggingSeverity, message: _InternalLogMessage): void;
        static warnToConsole(message: string): void;
        static resetInternalMessageCount(): void;
        static clearInternalMessageLoggedTypes(): void;
        static setMaxInternalMessageLimit(limit: number): void;
        private static logInternalMessage(severity, message);
        private static _areInternalMessagesThrottled();
    }
}
declare module M.Microsoft.ApplicationInsights {
    class Util {
        private static document;
        private static _canUseCookies;
        static NotSpecified: string;
        private static _getLocalStorageObject();
        private static _getVerifiedStorageObject(storageType);
        static canUseLocalStorage(): boolean;
        static getStorage(name: string): string;
        static setStorage(name: string, data: string): boolean;
        static removeStorage(name: string): boolean;
        private static _getSessionStorageObject();
        static canUseSessionStorage(): boolean;
        static getSessionStorageKeys(): string[];
        static getSessionStorage(name: string): string;
        static setSessionStorage(name: string, data: string): boolean;
        static removeSessionStorage(name: string): boolean;
        static canUseCookies(): any;
        static setCookie(name: any, value: any, domain?: any): void;
        static stringToBoolOrDefault(str: any): boolean;
        static getCookie(name: any): string;
        static deleteCookie(name: string): void;
        static trim(str: any): string;
        static newId(): string;
        static isArray(obj: any): boolean;
        static isError(obj: any): boolean;
        static isDate(obj: any): boolean;
        static toISOStringForIE8(date: Date): string;
        static getIEVersion(userAgentStr?: string): number;
        static msToTimeSpan(totalms: number): string;
        static isCrossOriginError(message: string, url: string, lineNumber: number, columnNumber: number, error: Error): boolean;
        static dump(object: any): string;
        static getExceptionName(object: any): string;
        static addEventHandler(eventName: string, callback: any): boolean;
    }
    class UrlHelper {
        private static document;
        private static htmlAnchorElement;
        static parseUrl(url: any): HTMLAnchorElement;
        static getAbsoluteUrl(url: any): string;
        static getPathName(url: any): string;
    }
}
declare module M.Microsoft.ApplicationInsights {
    class extensions {
        static IsNullOrUndefined(obj: any): boolean;
    }
    class stringUtils {
        static GetLength(strObject: any): number;
    }
    class dateTime {
        static Now: () => number;
        static GetDuration: (start: any, end: any) => any;
    }
    class EventHelper {
        static AttachEvent(obj: any, eventNameWithoutOn: any, handlerRef: any): boolean;
        static DetachEvent(obj: any, eventNameWithoutOn: any, handlerRef: any): void;
    }
}
declare module M.Microsoft.ApplicationInsights {
    class XHRMonitoringState {
        openDone: boolean;
        setRequestHeaderDone: boolean;
        sendDone: boolean;
        abortDone: boolean;
        onreadystatechangeCallbackAttached: boolean;
    }
    class ajaxRecord {
        completed: boolean;
        requestHeadersSize: any;
        ttfb: any;
        responseReceivingDuration: any;
        callbackDuration: any;
        ajaxTotalDuration: any;
        aborted: any;
        pageUrl: any;
        requestUrl: any;
        requestSize: number;
        method: any;
        status: any;
        requestSentTime: any;
        responseStartedTime: any;
        responseFinishedTime: any;
        callbackFinishedTime: any;
        endTime: any;
        originalOnreadystatechage: any;
        xhrMonitoringState: XHRMonitoringState;
        clientFailure: number;
        id: string;
        constructor(id: string);
        getAbsoluteUrl(): string;
        getPathName(): string;
        CalculateMetrics: () => void;
    }
}
declare module M.Microsoft.ApplicationInsights {
    interface XMLHttpRequestInstrumented extends XMLHttpRequest {
        ajaxData: ajaxRecord;
    }
    class AjaxMonitor {
        private appInsights;
        private initialized;
        private static instrumentedByAppInsightsName;
        private currentWindowHost;
        constructor(appInsights: Microsoft.ApplicationInsights.AppInsights);
        private Init();
        static DisabledPropertyName: string;
        private isMonitoredInstance(xhr, excludeAjaxDataValidation?);
        private supportsMonitoring();
        private instrumentOpen();
        private openHandler(xhr, method, url, async);
        private static getFailedAjaxDiagnosticsMessage(xhr);
        private instrumentSend();
        private sendHandler(xhr, content);
        private instrumentAbort();
        private attachToOnReadyStateChange(xhr);
        private onAjaxComplete(xhr);
    }
}
declare module M.Microsoft.ApplicationInsights {
    class HashCodeScoreGenerator {
        static INT_MAX_VALUE: number;
        private static MIN_INPUT_LENGTH;
        getHashCodeScore(key: string): number;
        getHashCode(input: string): number;
    }
}
declare module M.Microsoft.ApplicationInsights {
    interface ISerializable {
        aiDataContract: any;
    }
}
declare module M.Microsoft.ApplicationInsights {
    enum FieldType {
        Default = 0,
        Required = 1,
        Array = 2,
        Hidden = 4,
    }
    class Serializer {
        static serialize(input: ISerializable): string;
        private static _serializeObject(source, name);
        private static _serializeArray(sources, name);
        private static _serializeStringMap(map, expectedType, name);
    }
}
declare module M.Microsoft.Telemetry {
    class Base {
        baseType: string;
        constructor();
    }
}
declare module M.Microsoft.Telemetry {
    class Envelope {
        ver: number;
        name: string;
        time: string;
        sampleRate: number;
        seq: string;
        iKey: string;
        flags: number;
        deviceId: string;
        os: string;
        osVer: string;
        appId: string;
        appVer: string;
        userId: string;
        tags: any;
        data: Base;
        constructor();
    }
}
declare module M.Microsoft.ApplicationInsights.Telemetry.Common {
    class Envelope extends Microsoft.Telemetry.Envelope implements IEnvelope {
        aiDataContract: any;
        constructor(data: Microsoft.Telemetry.Base, name: string);
    }
}
declare module M.Microsoft.ApplicationInsights.Telemetry.Common {
    class Base extends Microsoft.Telemetry.Base implements ISerializable {
        aiDataContract: {};
    }
}
declare module M.AI {
    class ContextTagKeys {
        applicationVersion: string;
        applicationBuild: string;
        applicationTypeId: string;
        applicationId: string;
        deviceId: string;
        deviceIp: string;
        deviceLanguage: string;
        deviceLocale: string;
        deviceModel: string;
        deviceNetwork: string;
        deviceNetworkName: string;
        deviceOEMName: string;
        deviceOS: string;
        deviceOSVersion: string;
        deviceRoleInstance: string;
        deviceRoleName: string;
        deviceScreenResolution: string;
        deviceType: string;
        deviceMachineName: string;
        deviceVMName: string;
        locationIp: string;
        operationId: string;
        operationName: string;
        operationParentId: string;
        operationRootId: string;
        operationSyntheticSource: string;
        operationIsSynthetic: string;
        operationCorrelationVector: string;
        sessionId: string;
        sessionIsFirst: string;
        sessionIsNew: string;
        userAccountAcquisitionDate: string;
        userAccountId: string;
        userAgent: string;
        userId: string;
        userStoreRegion: string;
        userAuthUserId: string;
        userAnonymousUserAcquisitionDate: string;
        userAuthenticatedUserAcquisitionDate: string;
        sampleRate: string;
        cloudName: string;
        cloudRoleVer: string;
        cloudEnvironment: string;
        cloudLocation: string;
        cloudDeploymentUnit: string;
        serverDeviceOS: string;
        serverDeviceOSVer: string;
        internalSdkVersion: string;
        internalAgentVersion: string;
        internalDataCollectorReceivedTime: string;
        internalProfileId: string;
        internalProfileClassId: string;
        internalAccountId: string;
        internalApplicationName: string;
        internalInstrumentationKey: string;
        internalTelemetryItemId: string;
        internalApplicationType: string;
        internalRequestSource: string;
        internalFlowType: string;
        internalIsAudit: string;
        internalTrackingSourceId: string;
        internalTrackingType: string;
        internalIsDiagnosticExample: string;
        constructor();
    }
}
declare module M.Microsoft.ApplicationInsights.Context {
    interface IApplication {
        ver: string;
        build: string;
    }
}
declare module M.Microsoft.ApplicationInsights.Context {
    class Application implements IApplication {
        ver: string;
        build: string;
    }
}
declare module M.Microsoft.ApplicationInsights.Context {
    interface IDevice {
        type: string;
        id: string;
        oemName: string;
        model: string;
        network: number;
        resolution: string;
        locale: string;
        ip: string;
        language: string;
        os: string;
        osversion: string;
    }
}
declare module M.Microsoft.ApplicationInsights.Context {
    class Device implements IDevice {
        type: string;
        id: string;
        oemName: string;
        model: string;
        network: number;
        resolution: string;
        locale: string;
        ip: string;
        language: string;
        os: string;
        osversion: string;
        constructor();
    }
}
declare module M.Microsoft.ApplicationInsights.Context {
    interface IInternal {
        sdkVersion: string;
        agentVersion: string;
    }
}
declare module M.Microsoft.ApplicationInsights.Context {
    class Internal implements IInternal {
        sdkVersion: string;
        agentVersion: string;
        constructor();
    }
}
declare module M.Microsoft.ApplicationInsights.Context {
    interface ILocation {
        ip: string;
    }
}
declare module M.Microsoft.ApplicationInsights.Context {
    class Location implements ILocation {
        ip: string;
    }
}
declare module M.Microsoft.ApplicationInsights.Context {
    interface IOperation {
        id: string;
        name: string;
        parentId: string;
        rootId: string;
        syntheticSource: string;
    }
}
declare module M.Microsoft.ApplicationInsights.Context {
    class Operation implements IOperation {
        id: string;
        name: string;
        parentId: string;
        rootId: string;
        syntheticSource: string;
        constructor();
    }
}
declare module M.Microsoft.ApplicationInsights {
    class SamplingScoreGenerator {
        private hashCodeGeneragor;
        constructor();
        getSamplingScore(envelope: Microsoft.ApplicationInsights.IEnvelope): number;
    }
}
declare module M.Microsoft.ApplicationInsights.Context {
    interface ISample {
        sampleRate: number;
    }
}
declare module M.Microsoft.ApplicationInsights.Context {
    class Sample implements ISample {
        sampleRate: number;
        private samplingScoreGenerator;
        INT_MAX_VALUE: number;
        constructor(sampleRate: number);
        isSampledIn(envelope: Microsoft.ApplicationInsights.IEnvelope): boolean;
    }
}
declare module M.AI {
    enum SessionState {
        Start = 0,
        End = 1,
    }
}
declare module M.Microsoft.ApplicationInsights.Context {
    interface ISession {
        id: string;
        isFirst: boolean;
        acquisitionDate: number;
        renewalDate: number;
    }
}
declare module M.Microsoft.ApplicationInsights.Context {
    interface ISessionConfig {
        sessionRenewalMs: () => number;
        sessionExpirationMs: () => number;
        cookieDomain: () => string;
    }
    class Session implements ISession {
        id: string;
        isFirst: boolean;
        acquisitionDate: number;
        renewalDate: number;
    }
    class _SessionManager {
        static acquisitionSpan: number;
        static renewalSpan: number;
        automaticSession: Session;
        config: ISessionConfig;
        constructor(config: ISessionConfig);
        update(): void;
        backup(): void;
        private initializeAutomaticSession();
        private initializeAutomaticSessionWithData(sessionData);
        private renew();
        private setCookie(guid, acq, renewal);
        private setStorage(guid, acq, renewal);
    }
}
declare module M.Microsoft.ApplicationInsights.Context {
    interface IUser {
        config: any;
        id: string;
        authenticatedId: string;
        accountId: string;
        accountAcquisitionDate: string;
        agent: string;
        storeRegion: string;
    }
}
declare module M.Microsoft.ApplicationInsights.Context {
    class User implements IUser {
        static cookieSeparator: string;
        static userCookieName: string;
        static authUserCookieName: string;
        config: ITelemetryConfig;
        id: string;
        authenticatedId: string;
        accountId: string;
        accountAcquisitionDate: string;
        agent: string;
        storeRegion: string;
        setAuthenticatedUserContext(authenticatedUserId: string, accountId?: string): void;
        clearAuthenticatedUserContext(): void;
        constructor(config: ITelemetryConfig);
        private validateUserInput(id);
    }
}
declare module M.Microsoft.ApplicationInsights {
    class DataLossAnalyzer {
        static enabled: boolean;
        static appInsights: Microsoft.ApplicationInsights.AppInsights;
        static issuesReportedForThisSession: any;
        static itemsRestoredFromSessionBuffer: number;
        static LIMIT_PER_SESSION: number;
        static ITEMS_QUEUED_KEY: string;
        static ISSUES_REPORTED_KEY: string;
        static reset(): void;
        private static isEnabled();
        static getIssuesReported(): number;
        static incrementItemsQueued(): void;
        static decrementItemsQueued(countOfItemsSentSuccessfully: number): void;
        static getNumberOfLostItems(): number;
        static reportLostItems(): void;
    }
}
declare module M.Microsoft.ApplicationInsights {
    interface ISendBuffer {
        enqueue: (payload: string) => void;
        count: () => number;
        clear: () => void;
        getItems: () => string[];
        batchPayloads: (payload: string[]) => string;
        markAsSent: (payload: string[]) => void;
        clearSent: (payload: string[]) => void;
    }
    class ArraySendBuffer implements ISendBuffer {
        private _config;
        private _buffer;
        constructor(config: ISenderConfig);
        enqueue(payload: string): void;
        count(): number;
        clear(): void;
        getItems(): string[];
        batchPayloads(payload: string[]): string;
        markAsSent(payload: string[]): void;
        clearSent(payload: string[]): void;
    }
    class SessionStorageSendBuffer implements ISendBuffer {
        static BUFFER_KEY: string;
        static SENT_BUFFER_KEY: string;
        static MAX_BUFFER_SIZE: number;
        private _bufferFullMessageSent;
        private _buffer;
        private _config;
        constructor(config: ISenderConfig);
        enqueue(payload: string): void;
        count(): number;
        clear(): void;
        getItems(): string[];
        batchPayloads(payload: string[]): string;
        markAsSent(payload: string[]): void;
        clearSent(payload: string[]): void;
        private removePayloadsFromBuffer(payloads, buffer);
        private getBuffer(key);
        private setBuffer(key, buffer);
    }
}
interface XDomainRequest extends XMLHttpRequestEventTarget {
    responseText: string;
    send(payload: string): any;
    open(method: string, url: string): any;
}
declare var XDomainRequest: {
    prototype: XDomainRequest;
    new (): XDomainRequest;
};
declare module M.Microsoft.ApplicationInsights {
    interface ISenderConfig {
        endpointUrl: () => string;
        emitLineDelimitedJson: () => boolean;
        maxBatchSizeInBytes: () => number;
        maxBatchInterval: () => number;
        disableTelemetry: () => boolean;
        enableSessionStorageBuffer: () => boolean;
        disablePartialResponseHandler: () => boolean;
    }
    interface IResponseError {
        index: number;
        statusCode: number;
        message: string;
    }
    interface IBackendResponse {
        itemsReceived: number;
        itemsAccepted: number;
        errors: IResponseError[];
    }
    class Sender {
        private _consecutiveErrors;
        private _retryAt;
        private _lastSend;
        private _timeoutHandle;
        _buffer: ISendBuffer;
        _config: ISenderConfig;
        _sender: (payload: string[], isAsync: boolean) => void;
        _XMLHttpRequestSupported: boolean;
        constructor(config: ISenderConfig);
        send(envelope: Microsoft.ApplicationInsights.IEnvelope): void;
        private _setupTimer();
        private _getSizeInBytes(list);
        triggerSend(async?: boolean): void;
        private _setRetryTime();
        private _parseResponse(response);
        private _xhrSender(payload, isAsync);
        private _xdrSender(payload, isAsync);
        _xhrReadyStateChange(xhr: XMLHttpRequest, payload: string[], countOfItemsInPayload: number): void;
        _xdrOnLoad(xdr: XDomainRequest, payload: string[]): void;
        _onPartialSuccess(payload: string[], results: IBackendResponse): void;
        _onError(payload: string[], message: string, event?: ErrorEvent): void;
        _onSuccess(payload: string[], countOfItemsInPayload: number): void;
    }
}
declare module M.Microsoft.ApplicationInsights {
    class SplitTest {
        private hashCodeGeneragor;
        isEnabled(key: string, percentEnabled: number): boolean;
    }
}
declare module M.Microsoft.Telemetry {
    class Domain {
        constructor();
    }
}
declare module M.AI {
    enum SeverityLevel {
        Verbose = 0,
        Information = 1,
        Warning = 2,
        Error = 3,
        Critical = 4,
    }
}
declare module M.AI {
    class MessageData extends Microsoft.Telemetry.Domain {
        ver: number;
        message: string;
        severityLevel: AI.SeverityLevel;
        properties: any;
        constructor();
    }
}
declare module M.Microsoft.ApplicationInsights.Telemetry.Common {
    class DataSanitizer {
        private static MAX_NAME_LENGTH;
        private static MAX_STRING_LENGTH;
        private static MAX_URL_LENGTH;
        private static MAX_MESSAGE_LENGTH;
        private static MAX_EXCEPTION_LENGTH;
        static sanitizeKeyAndAddUniqueness(key: any, map: any): any;
        static sanitizeKey(name: any): any;
        static sanitizeString(value: any): any;
        static sanitizeUrl(url: any): any;
        static sanitizeMessage(message: any): any;
        static sanitizeException(exception: any): any;
        static sanitizeProperties(properties: any): any;
        static sanitizeMeasurements(measurements: any): any;
        static padNumber(num: any): string;
    }
}
declare module M.Microsoft.ApplicationInsights.Telemetry {
    class Trace extends AI.MessageData implements ISerializable {
        static envelopeType: string;
        static dataType: string;
        aiDataContract: {
            ver: FieldType;
            message: FieldType;
            severityLevel: FieldType;
            measurements: FieldType;
            properties: FieldType;
        };
        constructor(message: string, properties?: Object);
    }
}
declare module M.AI {
    class EventData extends Microsoft.Telemetry.Domain {
        ver: number;
        name: string;
        properties: any;
        measurements: any;
        constructor();
    }
}
declare module M.Microsoft.ApplicationInsights.Telemetry {
    class Event extends AI.EventData implements ISerializable {
        static envelopeType: string;
        static dataType: string;
        aiDataContract: {
            ver: FieldType;
            name: FieldType;
            properties: FieldType;
            measurements: FieldType;
        };
        constructor(name: string, properties?: Object, measurements?: Object);
    }
}
declare module M.AI {
    class ExceptionDetails {
        id: number;
        outerId: number;
        typeName: string;
        message: string;
        hasFullStack: boolean;
        stack: string;
        parsedStack: StackFrame[];
        constructor();
    }
}
declare module M.AI {
    class ExceptionData extends Microsoft.Telemetry.Domain {
        ver: number;
        handledAt: string;
        exceptions: ExceptionDetails[];
        severityLevel: AI.SeverityLevel;
        problemId: string;
        crashThreadId: number;
        properties: any;
        measurements: any;
        constructor();
    }
}
declare module M.AI {
    class StackFrame {
        level: number;
        method: string;
        assembly: string;
        fileName: string;
        line: number;
        constructor();
    }
}
declare module M.Microsoft.ApplicationInsights.Telemetry {
    class Exception extends AI.ExceptionData implements ISerializable {
        static envelopeType: string;
        static dataType: string;
        aiDataContract: {
            ver: FieldType;
            handledAt: FieldType;
            exceptions: FieldType;
            severityLevel: FieldType;
            properties: FieldType;
            measurements: FieldType;
        };
        constructor(exception: Error, handledAt?: string, properties?: Object, measurements?: Object, severityLevel?: AI.SeverityLevel);
        static CreateSimpleException(message: string, typeName: string, assembly: string, fileName: string, details: string, line: number, handledAt?: string): Telemetry.Exception;
    }
    class _StackFrame extends AI.StackFrame implements ISerializable {
        static regex: RegExp;
        static baseSize: number;
        sizeInBytes: number;
        aiDataContract: {
            level: FieldType;
            method: FieldType;
            assembly: FieldType;
            fileName: FieldType;
            line: FieldType;
        };
        constructor(frame: string, level: number);
    }
}
declare module M.AI {
    class MetricData extends Microsoft.Telemetry.Domain {
        ver: number;
        metrics: DataPoint[];
        properties: any;
        constructor();
    }
}
declare module M.AI {
    enum DataPointType {
        Measurement = 0,
        Aggregation = 1,
    }
}
declare module M.AI {
    class DataPoint {
        name: string;
        kind: AI.DataPointType;
        value: number;
        count: number;
        min: number;
        max: number;
        stdDev: number;
        constructor();
    }
}
declare module M.Microsoft.ApplicationInsights.Telemetry.Common {
    class DataPoint extends AI.DataPoint implements ISerializable {
        aiDataContract: {
            name: FieldType;
            kind: FieldType;
            value: FieldType;
            count: FieldType;
            min: FieldType;
            max: FieldType;
            stdDev: FieldType;
        };
    }
}
declare module M.Microsoft.ApplicationInsights.Telemetry {
    class Metric extends AI.MetricData implements ISerializable {
        static envelopeType: string;
        static dataType: string;
        aiDataContract: {
            ver: FieldType;
            metrics: FieldType;
            properties: FieldType;
        };
        constructor(name: string, value: number, count?: number, min?: number, max?: number, properties?: Object);
    }
}
declare module M.AI {
    class PageViewData extends AI.EventData {
        ver: number;
        url: string;
        name: string;
        duration: string;
        referrer: string;
        referrerData: string;
        properties: any;
        measurements: any;
        constructor();
    }
}
declare module M.Microsoft.ApplicationInsights.Telemetry {
    class PageView extends AI.PageViewData implements ISerializable {
        static envelopeType: string;
        static dataType: string;
        aiDataContract: {
            ver: FieldType;
            name: FieldType;
            url: FieldType;
            duration: FieldType;
            properties: FieldType;
            measurements: FieldType;
        };
        constructor(name?: string, url?: string, durationMs?: number, properties?: any, measurements?: any);
    }
}
declare module M.AI {
    class PageViewPerfData extends AI.PageViewData {
        ver: number;
        url: string;
        perfTotal: string;
        name: string;
        duration: string;
        networkConnect: string;
        referrer: string;
        sentRequest: string;
        referrerData: string;
        receivedResponse: string;
        domProcessing: string;
        properties: any;
        measurements: any;
        constructor();
    }
}
declare module M.Microsoft.ApplicationInsights.Telemetry {
    class PageViewPerformance extends AI.PageViewPerfData implements ISerializable {
        static envelopeType: string;
        static dataType: string;
        aiDataContract: {
            ver: FieldType;
            name: FieldType;
            url: FieldType;
            duration: FieldType;
            perfTotal: FieldType;
            networkConnect: FieldType;
            sentRequest: FieldType;
            receivedResponse: FieldType;
            domProcessing: FieldType;
            properties: FieldType;
            measurements: FieldType;
        };
        private isValid;
        getIsValid(): boolean;
        private durationMs;
        getDurationMs(): number;
        constructor(name: string, url: string, unused: number, properties?: any, measurements?: any);
        static getPerformanceTiming(): PerformanceTiming;
        static isPerformanceTimingSupported(): PerformanceTiming;
        static isPerformanceTimingDataReady(): boolean;
        static getDuration(start: any, end: any): number;
    }
}
declare module M.Microsoft.ApplicationInsights {
    interface IEnvelope extends ISerializable {
        ver: number;
        name: string;
        time: string;
        sampleRate: number;
        seq: string;
        iKey: string;
        flags: number;
        deviceId: string;
        os: string;
        osVer: string;
        appId: string;
        appVer: string;
        userId: string;
        tags: {
            [name: string]: any;
        };
    }
}
declare module M.Microsoft.ApplicationInsights {
    interface ITelemetryContext {
        application: Context.IApplication;
        device: Context.IDevice;
        internal: Context.IInternal;
        location: Context.ILocation;
        operation: Context.IOperation;
        sample: Context.ISample;
        user: Context.IUser;
        session: Context.ISession;
        addTelemetryInitializer(telemetryInitializer: (envelope: Microsoft.ApplicationInsights.IEnvelope) => boolean): any;
        track(envelope: Microsoft.ApplicationInsights.IEnvelope): any;
    }
}
declare module M.Microsoft.ApplicationInsights {
    interface ITelemetryConfig extends ISenderConfig {
        instrumentationKey: () => string;
        accountId: () => string;
        sessionRenewalMs: () => number;
        sessionExpirationMs: () => number;
        sampleRate: () => number;
        endpointUrl: () => string;
        cookieDomain: () => string;
    }
    class TelemetryContext implements ITelemetryContext {
        _config: ITelemetryConfig;
        _sender: Sender;
        application: Context.Application;
        device: Context.Device;
        internal: Context.Internal;
        location: Context.Location;
        operation: Context.Operation;
        sample: Context.Sample;
        user: Context.User;
        session: Context.Session;
        private telemetryInitializers;
        _sessionManager: Microsoft.ApplicationInsights.Context._SessionManager;
        constructor(config: ITelemetryConfig);
        addTelemetryInitializer(telemetryInitializer: (envelope: Microsoft.ApplicationInsights.IEnvelope) => boolean): void;
        track(envelope: Microsoft.ApplicationInsights.IEnvelope): IEnvelope;
        private _track(envelope);
        private _applyApplicationContext(envelope, appContext);
        private _applyDeviceContext(envelope, deviceContext);
        private _applyInternalContext(envelope, internalContext);
        private _applyLocationContext(envelope, locationContext);
        private _applyOperationContext(envelope, operationContext);
        private _applySampleContext(envelope, sampleContext);
        private _applySessionContext(envelope, sessionContext);
        private _applyUserContext(envelope, userContext);
    }
}
declare module M.Microsoft.Telemetry {
    class Data<TDomain> extends Microsoft.Telemetry.Base {
        baseType: string;
        baseData: TDomain;
        constructor();
    }
}
declare module M.Microsoft.ApplicationInsights.Telemetry.Common {
    class Data<TDomain> extends Microsoft.Telemetry.Data<TDomain> implements ISerializable {
        aiDataContract: {
            baseType: FieldType;
            baseData: FieldType;
        };
        constructor(type: string, data: TDomain);
    }
}
declare module M.Microsoft.ApplicationInsights.Telemetry {
    class PageViewManager {
        private pageViewPerformanceSent;
        private overridePageViewDuration;
        private appInsights;
        constructor(appInsights: IAppInsightsInternal, overridePageViewDuration: boolean);
        trackPageView(name?: string, url?: string, properties?: Object, measurements?: Object, duration?: number): void;
    }
}
declare module M.Microsoft.ApplicationInsights.Telemetry {
    class PageVisitTimeManager {
        private prevPageVisitDataKeyName;
        private pageVisitTimeTrackingHandler;
        constructor(pageVisitTimeTrackingHandler: (pageName: string, pageUrl: string, pageVisitTime: number) => void);
        trackPreviousPageVisit(currentPageName: string, currentPageUrl: string): void;
        restartPageVisitTimer(pageName: string, pageUrl: string): PageVisitData;
        startPageVisitTimer(pageName: string, pageUrl: string): void;
        stopPageVisitTimer(): PageVisitData;
    }
    class PageVisitData {
        pageName: string;
        pageUrl: string;
        pageVisitStartTime: number;
        pageVisitTime: number;
        constructor(pageName: any, pageUrl: any);
    }
}
declare module M.AI {
    enum DependencyKind {
        SQL = 0,
        Http = 1,
        Other = 2,
    }
}
declare module M.AI {
    enum DependencySourceType {
        Undefined = 0,
        Aic = 1,
        Apmc = 2,
    }
}
declare module M.AI {
    class RemoteDependencyData extends Microsoft.Telemetry.Domain {
        ver: number;
        name: string;
        id: string;
        resultCode: string;
        kind: AI.DataPointType;
        value: number;
        count: number;
        min: number;
        max: number;
        stdDev: number;
        dependencyKind: AI.DependencyKind;
        success: boolean;
        async: boolean;
        dependencySource: AI.DependencySourceType;
        commandName: string;
        dependencyTypeName: string;
        properties: any;
        constructor();
    }
}
declare module M.Microsoft.ApplicationInsights.Telemetry {
    class RemoteDependencyData extends AI.RemoteDependencyData implements ISerializable {
        static envelopeType: string;
        static dataType: string;
        aiDataContract: {
            id: FieldType;
            ver: FieldType;
            name: FieldType;
            kind: FieldType;
            value: FieldType;
            count: FieldType;
            min: FieldType;
            max: FieldType;
            stdDev: FieldType;
            dependencyKind: FieldType;
            success: FieldType;
            async: FieldType;
            dependencySource: FieldType;
            commandName: FieldType;
            dependencyTypeName: FieldType;
            properties: FieldType;
            resultCode: FieldType;
        };
        constructor(id: string, absoluteUrl: string, commandName: string, value: number, success: boolean, resultCode: number, method?: string);
        private formatDependencyName(method, absoluteUrl);
    }
}
declare module M.Microsoft.ApplicationInsights {
    interface IConfig {
        instrumentationKey?: string;
        endpointUrl?: string;
        emitLineDelimitedJson?: boolean;
        accountId?: string;
        sessionRenewalMs?: number;
        sessionExpirationMs?: number;
        maxBatchSizeInBytes?: number;
        maxBatchInterval?: number;
        enableDebug?: boolean;
        disableExceptionTracking?: boolean;
        disableTelemetry?: boolean;
        verboseLogging?: boolean;
        diagnosticLogInterval?: number;
        samplingPercentage?: number;
        autoTrackPageVisitTime?: boolean;
        disableAjaxTracking?: boolean;
        overridePageViewDuration?: boolean;
        maxAjaxCallsPerView?: number;
        disableDataLossAnalysis?: boolean;
        disableCorrelationHeaders?: boolean;
        disableFlushOnBeforeUnload?: boolean;
        enableSessionStorageBuffer?: boolean;
        cookieDomain?: string;
        disablePartialResponseHandler?: boolean;
        url?: string;
    }
}
declare module M.Microsoft.ApplicationInsights {
    interface IAppInsights {
        config: IConfig;
        context: ITelemetryContext;
        queue: (() => void)[];
        startTrackPage(name?: string): any;
        stopTrackPage(name?: string, url?: string, properties?: {
            [name: string]: string;
        }, measurements?: {
            [name: string]: number;
        }): any;
        trackPageView(name?: string, url?: string, properties?: {
            [name: string]: string;
        }, measurements?: {
            [name: string]: number;
        }, duration?: number): any;
        startTrackEvent(name: string): any;
        stopTrackEvent(name: string, properties?: {
            [name: string]: string;
        }, measurements?: {
            [name: string]: number;
        }): any;
        trackEvent(name: string, properties?: {
            [name: string]: string;
        }, measurements?: {
            [name: string]: number;
        }): any;
        trackDependency(id: string, method: string, absoluteUrl: string, pathName: string, totalTime: number, success: boolean, resultCode: number): any;
        trackException(exception: Error, handledAt?: string, properties?: {
            [name: string]: string;
        }, measurements?: {
            [name: string]: number;
        }, severityLevel?: AI.SeverityLevel): any;
        trackMetric(name: string, average: number, sampleCount?: number, min?: number, max?: number, properties?: {
            [name: string]: string;
        }): any;
        trackTrace(message: string, properties?: {
            [name: string]: string;
        }): any;
        flush(): any;
        setAuthenticatedUserContext(authenticatedUserId: string, accountId?: string): any;
        clearAuthenticatedUserContext(): any;
        downloadAndSetup?(config: Microsoft.ApplicationInsights.IConfig): void;
        _onerror(message: string, url: string, lineNumber: number, columnNumber: number, error: Error): any;
    }
}
declare module M.Microsoft.ApplicationInsights {
    var Version: string;
    interface IAppInsightsInternal {
        sendPageViewInternal(name?: string, url?: string, duration?: number, properties?: Object, measurements?: Object): any;
        sendPageViewPerformanceInternal(pageViewPerformance: ApplicationInsights.Telemetry.PageViewPerformance): any;
        flush(): any;
    }
    class AppInsights implements IAppInsightsInternal, IAppInsights {
        private _trackAjaxAttempts;
        private _eventTracking;
        private _pageTracking;
        private _pageViewManager;
        private _pageVisitTimeManager;
        config: IConfig;
        context: TelemetryContext;
        queue: (() => void)[];
        static defaultConfig: IConfig;
        constructor(config: IConfig);
        sendPageViewInternal(name?: string, url?: string, duration?: number, properties?: Object, measurements?: Object): void;
        sendPageViewPerformanceInternal(pageViewPerformance: ApplicationInsights.Telemetry.PageViewPerformance): void;
        startTrackPage(name?: string): void;
        stopTrackPage(name?: string, url?: string, properties?: Object, measurements?: Object): void;
        trackPageView(name?: string, url?: string, properties?: Object, measurements?: Object, duration?: number): void;
        startTrackEvent(name: string): void;
        stopTrackEvent(name: string, properties?: Object, measurements?: Object): void;
        trackEvent(name: string, properties?: Object, measurements?: Object): void;
        trackDependency(id: string, method: string, absoluteUrl: string, pathName: string, totalTime: number, success: boolean, resultCode: number): void;
        trackAjax(id: string, absoluteUrl: string, pathName: string, totalTime: number, success: boolean, resultCode: number, method?: string): void;
        trackException(exception: Error, handledAt?: string, properties?: Object, measurements?: Object, severityLevel?: AI.SeverityLevel): void;
        trackMetric(name: string, average: number, sampleCount?: number, min?: number, max?: number, properties?: Object): void;
        trackTrace(message: string, properties?: Object): void;
        private trackPageVisitTime(pageName, pageUrl, pageVisitTime);
        flush(): void;
        setAuthenticatedUserContext(authenticatedUserId: string, accountId?: string): void;
        clearAuthenticatedUserContext(): void;
        private SendCORSException(properties);
        _onerror(message: string, url: string, lineNumber: number, columnNumber: number, error: Error): void;
    }
}
declare module M.Microsoft.ApplicationInsights {
    interface Snippet {
        queue: Array<() => void>;
        config: IConfig;
    }
    class Initialization {
        snippet: Snippet;
        config: IConfig;
        constructor(snippet: Snippet);
        loadAppInsights(): AppInsights;
        emptyQueue(): void;
        pollInteralLogs(appInsightsInstance: AppInsights): number;
        addHousekeepingBeforeUnload(appInsightsInstance: AppInsights): void;
        static getDefaultConfig(config?: IConfig): IConfig;
    }
}
declare module M.Microsoft.ApplicationInsights {
}
declare module M.AI {
    class AjaxCallData extends AI.PageViewData {
        ver: number;
        url: string;
        ajaxUrl: string;
        name: string;
        duration: string;
        requestSize: number;
        referrer: string;
        responseSize: number;
        referrerData: string;
        timeToFirstByte: string;
        timeToLastByte: string;
        callbackDuration: string;
        responseCode: string;
        success: boolean;
        properties: any;
        measurements: any;
        constructor();
    }
}
declare module M.AI {
    class RequestData extends Microsoft.Telemetry.Domain {
        ver: number;
        id: string;
        name: string;
        startTime: string;
        duration: string;
        responseCode: string;
        success: boolean;
        httpMethod: string;
        url: string;
        properties: any;
        measurements: any;
        constructor();
    }
}
declare module M.AI {
    class SessionStateData extends Microsoft.Telemetry.Domain {
        ver: number;
        state: AI.SessionState;
        constructor();
    }
}
declare module M.AI {
    enum TestResult {
        Pass = 0,
        Fail = 1,
    }
}

declare module "ApplicationInsights" {
	export = M;
}
